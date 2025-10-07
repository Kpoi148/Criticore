from fastapi import FastAPI, HTTPException, UploadFile, File, BackgroundTasks
from fastapi.responses import JSONResponse
from fastapi.middleware.cors import CORSMiddleware 
from pydantic import BaseModel, Field
from typing import Optional, Dict, Any, List
import tempfile
import os
import hashlib
import asyncio
from datetime import datetime
from concurrent.futures import ThreadPoolExecutor
import uvicorn

from rag import RAG
from config import Config
from monitoring_service import monitor

app = FastAPI(
    title="RAG API", 
    description="Retrieval-Augmented Generation API for Node.js Integration",
    version="1.0.0"
)
# Add CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # For development only, restrict this in production
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# Global instances
rag_system = RAG()
executor = ThreadPoolExecutor(max_workers=4)

# Pydantic Models
class UserProfile(BaseModel):
    user_id: str
    preferences: Optional[Dict[str, Any]] = {}
    context: Optional[str] = None

class QueryRequest(BaseModel):
    prompt: str = Field(..., description="The user's question or prompt")
    user_profile: Optional[UserProfile] = None
    k: Optional[int] = Field(4, description="Number of relevant documents to retrieve")
    temperature: Optional[float] = Field(0.7, description="LLM temperature for response generation")

class DocumentMetadata(BaseModel):
    filename: str
    document_hash: str
    upload_time: datetime
    chunk_count: int
    
class QueryResponse(BaseModel):
    answer: str
    sources: List[Dict[str, Any]]
    user_id: Optional[str] = None
    processing_time: float
    timestamp: datetime

class DocumentUploadResponse(BaseModel):
    message: str
    document_metadata: DocumentMetadata
    processing_time: float

# Helper Functions
def calculate_document_hash(content: bytes) -> str:
    """Calculate SHA-256 hash of document content"""
    return hashlib.sha256(content).hexdigest()

def enhance_prompt_with_user_profile(prompt: str, user_profile: Optional[UserProfile]) -> str:
    """Enhance the prompt with user profile context"""
    if not user_profile:
        return prompt
    
    enhanced_prompt = prompt
    
    if user_profile.context:
        enhanced_prompt = f"Context: {user_profile.context}\n\nQuestion: {enhanced_prompt}"
    
    if user_profile.preferences:
        prefs = ", ".join([f"{k}: {v}" for k, v in user_profile.preferences.items()])
        enhanced_prompt = f"User preferences: {prefs}\n\n{enhanced_prompt}"
    
    return enhanced_prompt

async def process_document_async(file_content: bytes, filename: str) -> DocumentUploadResponse:
    """Process document upload asynchronously"""
    loop = asyncio.get_event_loop()
    
    def process_sync():
        start_time = datetime.now()
        tmp_file_path = None
        
        try:
            # Save temporary file
            with tempfile.NamedTemporaryFile(delete=False, suffix='.pdf') as tmp_file:
                tmp_file.write(file_content)
                tmp_file_path = tmp_file.name
            
            # Process document - this now returns a dict
            result = rag_system.load_documents(tmp_file_path)
            
            # Calculate metadata
            doc_hash = calculate_document_hash(file_content)
            chunk_count = result.get("count", 0) if isinstance(result, dict) else 0
            
            processing_time = (datetime.now() - start_time).total_seconds()
            
            return DocumentUploadResponse(
                message=result.get("message", "Document processed successfully") if isinstance(result, dict) else "Document processed successfully",
                document_metadata=DocumentMetadata(
                    filename=filename,
                    document_hash=doc_hash,
                    upload_time=start_time,
                    chunk_count=chunk_count
                ),
                processing_time=processing_time
            )
        
        except Exception as e:
            raise Exception(f"Failed to process document: {str(e)}")
        finally:
            # Clean up temporary file
            if tmp_file_path and os.path.exists(tmp_file_path):
                try:
                    os.unlink(tmp_file_path)
                except Exception as cleanup_error:
                    print(f"Warning: Failed to cleanup temp file: {cleanup_error}")
    
    return await loop.run_in_executor(executor, process_sync)

# API Endpoints
@app.get("/")
async def root():
    return {
        "message": "RAG API is running",
        "version": "1.0.0",
        "status": "healthy",
        "embedding_provider": Config.EMBEDDING_PROVIDER
    }

@app.get("/health")
async def health_check():
    return {
        "status": "healthy",
        "initialized": rag_system.is_initialized,
        "embedding_provider": Config.EMBEDDING_PROVIDER,
        "timestamp": datetime.now()
    }

@app.on_event("startup")
async def startup_event():
    """Initialize services on startup"""
    # Start monitoring service
    monitor.start_monitoring(interval=30)  # 30-second interval
    print("Application startup complete")

@app.on_event("shutdown")
async def shutdown_event():
    """Cleanup on shutdown"""
    # Stop monitoring
    monitor.stop_monitoring()
    print("Application shutdown complete")

# Add new endpoint for performance metrics
@app.get("/metrics")
async def get_performance_metrics():
    """Get system performance metrics"""
    return monitor.get_performance_report()

@app.post("/optimize-memory")
async def optimize_memory():
    """Force memory optimization"""
    return monitor.optimize_memory(force=True)

# Update existing endpoints to log metrics
@app.post("/upload", response_model=DocumentUploadResponse)
async def upload_document(file: UploadFile = File(...)):
    """Upload and process a PDF document"""
    # Validate filename exists
    if not file.filename:
        raise HTTPException(status_code=400, detail="Filename is required")
    
    # Validate file extension properly
    if not Config.validate_file_extension(file.filename):
        raise HTTPException(
            status_code=400, 
            detail=f"Unsupported file format. Allowed: {', '.join(Config.ALLOWED_EXTENSIONS)}"
        )
    
    # Validate file size
    try:
        content = await file.read()
        if len(content) == 0:
            raise HTTPException(status_code=400, detail="Empty file uploaded")
        
        if len(content) > Config.MAX_FILE_SIZE:
            raise HTTPException(status_code=400, detail=f"File too large. Max size: {Config.MAX_FILE_SIZE} bytes")
        
        # Calculate hash before processing
        doc_hash = calculate_document_hash(content)
        
        response = await process_document_async(content, file.filename)
        
        # Fix: Handle the new return type from rag.load_documents
        result = rag_system.document_service.get_processed_documents_info()
        response.document_metadata.chunk_count = result["total_chunks"]
        
        # Add duplicate detection info
        if response.document_metadata.chunk_count == 0:
            response.message = f"Document already exists (hash: {doc_hash[:8]}...)"
        
        # Log document processing metrics
        monitor.log_document_processing(
            filename=file.filename,
            doc_size=len(content),
            processing_time=response.processing_time,
            chunk_count=response.document_metadata.chunk_count
        )
        
        return response
    
    except HTTPException:
        raise
    except Exception as e:
        # Ensure temp file is always cleaned up
        if 'tmp_file_path' in locals() and tmp_file_path and os.path.exists(tmp_file_path):
            try:
                os.unlink(tmp_file_path)
            except:
                pass
        raise HTTPException(status_code=500, detail=f"Document processing failed: {str(e)}")

# Add new endpoint to check document status
@app.get("/documents/status")
async def get_documents_status():
    """Get information about processed documents"""
    try:
        info = rag_system.document_service.get_processed_documents_info()
        return {
            "processed_documents": info["processed_count"],
            "total_chunks": info["total_chunks"],
            "document_hashes": [h[:8] + "..." for h in info["document_hashes"]]
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    
@app.post("/query", response_model=QueryResponse)
async def query_documents(request: QueryRequest):
    """Query the RAG system with user profile support"""
    if not rag_system.is_initialized:
        raise HTTPException(
            status_code=400, 
            detail="No documents loaded. Upload a PDF document first."
        )
    
    try:
        start_time = datetime.now()
        
        # Enhance prompt with user profile
        enhanced_prompt = enhance_prompt_with_user_profile(request.prompt, request.user_profile)
        
        # Process query
        loop = asyncio.get_event_loop()
        
        def query_sync():
            return rag_system.query(enhanced_prompt, k=request.k) # type: ignore
        
        response = await loop.run_in_executor(executor, query_sync)
        
        processing_time = (datetime.now() - start_time).total_seconds()
        
        # Format response
        if isinstance(response, str):
            return QueryResponse(
                answer=response,
                sources=[],
                user_id=request.user_profile.user_id if request.user_profile else None,
                processing_time=processing_time,
                timestamp=datetime.now()
            )
        
        # Extract source information
        sources = []
        if 'context' in response:
            for i, doc in enumerate(response['context']):
                sources.append({
                    "source_id": i + 1,
                    "content": doc.page_content[:300] + "..." if len(doc.page_content) > 300 else doc.page_content,
                    "metadata": doc.metadata
                })
        
        # Log query metrics
        monitor.log_query_performance(
            query_length=len(request.prompt),
            retrieved_chunks=len(sources),
            processing_time=processing_time
        )
        
        return QueryResponse(
            answer=response.get('answer', 'No answer found'),
            sources=sources,
            user_id=request.user_profile.user_id if request.user_profile else None,
            processing_time=processing_time,
            timestamp=datetime.now()
        )
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Query processing failed: {str(e)}")

@app.post("/chat")
async def chat_with_context(request: QueryRequest):
    """Simplified chat endpoint for Node.js integration"""
    try:
        query_response = await query_documents(request)
        
        # Return simplified JSON for easier Node.js handling
        return {
            "response": query_response.answer,
            "sources": len(query_response.sources),
            "user_id": query_response.user_id,
            "timestamp": query_response.timestamp.isoformat(),
            "processing_time_ms": round(query_response.processing_time * 1000)
        }
    
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

# Error Handlers
@app.exception_handler(HTTPException)
async def http_exception_handler(request, exc):
    return JSONResponse(
        status_code=exc.status_code,
        content={
            "error": "HTTP Exception",
            "detail": exc.detail,
            "timestamp": datetime.now().isoformat()
        }
    )

@app.exception_handler(Exception)
async def global_exception_handler(request, exc):
    return JSONResponse(
        status_code=500,
        content={
            "error": "Internal server error",
            "detail": str(exc),
            "timestamp": datetime.now().isoformat()
        }
    )

if __name__ == "__main__":
    uvicorn.run("app:app", host="0.0.0.0", port=8000, reload=True)