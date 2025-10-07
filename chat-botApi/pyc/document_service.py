from langchain_community.document_loaders import PyPDFLoader
from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain_community.vectorstores import FAISS
from typing import Optional, Set, Dict, Any, List
import os
import hashlib
import gc

from embeddings_service import create_embeddings
from config import Config

class DocumentService:
    def __init__(self, persist_directory: str = "vectorstore"):
        self.text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=Config.CHUNK_SIZE, 
            chunk_overlap=Config.CHUNK_OVERLAP
        )
        self.embeddings = None
        self.vectorstore: Optional[FAISS] = None
        self.processed_documents: Set[str] = set()
        self.persist_directory = persist_directory
        
        # Try to load existing vectorstore
        self._load_vectorstore()
    
    def _load_vectorstore(self):
        """Load vectorstore from disk if exists"""
        try:
            if os.path.exists(self.persist_directory):
                self._initialize_embeddings()
                print(f"Loading vectorstore from {self.persist_directory}")
                self.vectorstore = FAISS.load_local(
                    self.persist_directory, 
                    self.embeddings # type: ignore
                )
                # Also load processed documents list
                docs_file = os.path.join(self.persist_directory, "processed_docs.txt")
                if os.path.exists(docs_file):
                    with open(docs_file, 'r') as f:
                        self.processed_documents = set(line.strip() for line in f)
                print(f"✓ Loaded vectorstore with {len(self.vectorstore.index_to_docstore_id) if self.vectorstore else 0} chunks")
        except Exception as e:
            print(f"Warning: Failed to load vectorstore: {str(e)}")
            # Continue with empty vectorstore
    
    def _save_vectorstore(self):
        """Save vectorstore to disk"""
        if self.vectorstore:
            try:
                # Create directory if not exists
                os.makedirs(self.persist_directory, exist_ok=True)
                
                # Save vectorstore
                self.vectorstore.save_local(self.persist_directory)
                
                # Save processed documents list
                docs_file = os.path.join(self.persist_directory, "processed_docs.txt")
                with open(docs_file, 'w') as f:
                    for doc_hash in self.processed_documents:
                        f.write(f"{doc_hash}\n")
                
                print(f"✓ Saved vectorstore to {self.persist_directory}")
            except Exception as e:
                print(f"Warning: Failed to save vectorstore: {str(e)}")
    
    def _initialize_embeddings(self):
        """Initialize embeddings once"""
        if self.embeddings is None:
            try:
                self.embeddings = create_embeddings()
            except Exception as e:
                raise Exception(f"Failed to initialize embeddings: {str(e)}")
    
    def _calculate_file_hash(self, file_path: str) -> str:
        """Calculate SHA-256 hash of file content"""
        hash_sha256 = hashlib.sha256()
        with open(file_path, "rb") as f:
            for chunk in iter(lambda: f.read(4096), b""):
                hash_sha256.update(chunk)
        return hash_sha256.hexdigest()
    
    def add_documents(self, pdf_path: str) -> int:
        """Process and add documents to vectorstore (with duplicate prevention)"""
        # Validate file exists and is readable
        if not os.path.isfile(pdf_path):
            raise FileNotFoundError(f"File not found or not a file: {pdf_path}")
        
        # Calculate file hash to check for duplicates
        try:
            file_hash = self._calculate_file_hash(pdf_path)
        except Exception as e:
            raise Exception(f"Failed to calculate file hash: {str(e)}")
        
        if file_hash in self.processed_documents:
            print(f"   ⚠️  Document already processed (hash: {file_hash[:8]}...)")
            return 0  # Return 0 to indicate no new documents were added
        
        try:
            self._initialize_embeddings()
            
            # Load PDF with error handling
            loader = PyPDFLoader(pdf_path)
            documents = loader.load()
            
            if not documents:
                raise ValueError("No content extracted from PDF")
            
            # Split documents
            splits = self.text_splitter.split_documents(documents)
            
            if not splits:
                raise ValueError("No text chunks created from document")
            
            # Create or update vectorstore
            if self.vectorstore is None:
                self.vectorstore = FAISS.from_documents(splits, self.embeddings) # type: ignore
            else:
                self.vectorstore.add_documents(splits)
            
            # Mark this document as processed
            self.processed_documents.add(file_hash)
            print(f"   ✅ New document processed (hash: {file_hash[:8]}...) with {len(splits)} chunks")
            
            # Save vectorstore after successful update
            self._save_vectorstore()
            
            # Force garbage collection to free memory
            gc.collect()
            
            return len(documents)
        
        except Exception as e:
            # Don't mark as processed if there was an error
            raise Exception(f"Failed to process document: {str(e)}")

    def get_retriever(self, k: int = 4):
        """Get retriever for querying"""
        if not self.vectorstore:
            raise ValueError("No documents loaded")
        return self.vectorstore.as_retriever(search_kwargs={"k": k})
    
    def get_processed_documents_info(self) -> dict:
        """Get information about processed documents"""
        return {
            "processed_count": len(self.processed_documents),
            "document_hashes": list(self.processed_documents),
            "total_chunks": len(self.vectorstore.index_to_docstore_id) if self.vectorstore else 0
        }