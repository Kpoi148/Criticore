import os
from dotenv import load_dotenv

load_dotenv()

class Config:
    # Required API keys
    CEREBRAS_API_KEY = os.getenv("CEREBRAS_API_KEY")
    GOOGLE_API_KEY = os.getenv("GOOGLE_API_KEY")
    
    # Simplified to only Google embeddings for production
    EMBEDDING_PROVIDER = "google"
    GOOGLE_EMBEDDING_MODEL = "text-embedding-004"
    
    # LLM settings
    LLM_MODEL = "llama-3.3-70b"  # Faster model for production
    
    # Document processing
    CHUNK_SIZE = 2000  # Smaller chunks for better retrieval
    CHUNK_OVERLAP = 300
    
    # API settings
    MAX_FILE_SIZE = 10 * 1024 * 1024  # 10MB
    ALLOWED_EXTENSIONS = ['.pdf']
    
    # Performance settings
    MAX_CONCURRENT_REQUESTS = int(os.getenv("MAX_CONCURRENT_REQUESTS", "4"))
    VECTORSTORE_DIR = os.getenv("VECTORSTORE_DIR", "vectorstore")
    ENABLE_VECTORSTORE_PERSISTENCE = os.getenv("ENABLE_VECTORSTORE_PERSISTENCE", "true").lower() == "true"
    
    # Memory management
    MAX_CONTEXT_CHUNKS = int(os.getenv("MAX_CONTEXT_CHUNKS", "6"))  # Max chunks to send to LLM
    GC_AFTER_DOCUMENT_LOAD = os.getenv("GC_AFTER_DOCUMENT_LOAD", "true").lower() == "true"
    
    @classmethod
    def validate(cls):
        """Validate configuration on startup"""
        missing_keys = []
        
        if not cls.CEREBRAS_API_KEY:
            missing_keys.append("CEREBRAS_API_KEY")
        if not cls.GOOGLE_API_KEY:
            missing_keys.append("GOOGLE_API_KEY")
            
        if missing_keys:
            raise ValueError(f"Missing required environment variables: {', '.join(missing_keys)}")
    
    @classmethod
    def validate_file_size(cls, size: int) -> bool:
        """Validate file size"""
        return 0 < size <= cls.MAX_FILE_SIZE
    
    @classmethod
    def validate_file_extension(cls, filename: str) -> bool:
        """Validate file extension"""
        if not filename:
            return False
        ext = os.path.splitext(filename.lower())[1]
        return ext in cls.ALLOWED_EXTENSIONS