from langchain_google_genai import GoogleGenerativeAIEmbeddings
from config import Config
import functools

# Use singleton pattern for embeddings to avoid recreating
@functools.lru_cache(maxsize=1)
def create_embeddings():
    """Factory function to create production embeddings (singleton)"""
    print("Creating embedding model instance (cached)")
    return GoogleGenerativeAIEmbeddings(
        model=f"models/{Config.GOOGLE_EMBEDDING_MODEL}",
        google_api_key=Config.GOOGLE_API_KEY, # type: ignore
        task_type="retrieval_document"
    )