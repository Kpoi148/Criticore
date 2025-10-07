from document_service import DocumentService
from llm_service import LLMService
from typing import Optional, Union, Dict, Any, List
import time

class RAG:
    def __init__(self):
        print("Initializing RAG system...")
        start_time = time.time()
        
        try:
            # Only initialize services that don't require external connections
            self.document_service = DocumentService()
            self.llm_service = None  # Lazy initialization
            self.is_initialized = False
            
            init_time = time.time() - start_time
            print(f"✓ RAG system initialized in {init_time:.2f} seconds")
        except Exception as e:
            print(f"✗ Failed to initialize RAG system: {str(e)}")
            raise
    
    def _initialize_llm(self):
        """Lazy initialization of LLM service"""
        if self.llm_service is None:
            try:
                print("Initializing LLM service...")
                start_time = time.time()
                self.llm_service = LLMService()
                init_time = time.time() - start_time
                print(f"✓ LLM service initialized in {init_time:.2f} seconds")
            except Exception as e:
                print(f"✗ Failed to initialize LLM service: {str(e)}")
                raise
    
    def load_documents(self, pdf_path: str) -> Dict[str, Any]:
        """Load documents and create vectorstore"""
        try:
            print(f"\nLoading documents from: {pdf_path}")
            start_time = time.time()
            
            count = self.document_service.add_documents(pdf_path)
            self.is_initialized = True
            
            load_time = time.time() - start_time
            message = f"Successfully loaded {count} documents from {pdf_path} in {load_time:.2f} seconds"
            print(message)
            return {"status": "success", "message": message, "count": count}
            
        except Exception as e:
            error_msg = f"Error loading documents: {str(e)}"
            print(error_msg)
            self.is_initialized = False
            raise Exception(error_msg)
    
    def query(self, question: str, k: int = 6) -> Dict[str, Any]:
        """Query the RAG system"""
        if not self.is_initialized:
            raise ValueError("No documents loaded. Please load documents first.")
        
        try:
            print(f"\nProcessing query: {question}")
            start_time = time.time()
            
            self._initialize_llm()  # Initialize LLM only when needed
            retriever = self.document_service.get_retriever(k=k)
            response = self.llm_service.generate_response(question, retriever) # type: ignore
            
            query_time = time.time() - start_time
            print(f"✓ Query processed in {query_time:.2f} seconds")
            
            # Ensure we always return a dictionary
            if isinstance(response, str):
                response = {"answer": response, "context": []}
            
            return response
        except Exception as e:
            error_msg = f"Error processing query: {str(e)}"
            print(error_msg)
            raise Exception(error_msg)
    
    def run(self):
        """Legacy method for backward compatibility"""
        print("RAG system ready")
        print("Available methods:")
        print("- load_documents(pdf_path): Load PDF documents")
        print("- query(question): Ask questions about loaded documents")