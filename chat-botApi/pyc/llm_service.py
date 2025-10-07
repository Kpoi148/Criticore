from langchain_cerebras import ChatCerebras
from langchain.prompts import ChatPromptTemplate
from langchain.chains.combine_documents import create_stuff_documents_chain
from langchain.chains import create_retrieval_chain

from config import Config

class LLMService:
    def __init__(self):
        Config.validate()
        self.llm = ChatCerebras(
            model=Config.LLM_MODEL,
            api_key=Config.CEREBRAS_API_KEY # type: ignore
        )
        self.prompt = self._create_prompt()
    
    def _create_prompt(self):
        """Create the RAG prompt template"""
        system_prompt = (
            "You are an expert assistant for question-answering tasks. "
            "Use the following retrieved context to answer the user's question. "
            "If you cannot find the answer in the context, state that you don't know."
            "\n\n{context}"
        )
        return ChatPromptTemplate.from_messages([
            ("system", system_prompt),
            ("human", "{input}"),
        ])
    
    def create_rag_chain(self, retriever):
        """Create the complete RAG chain"""
        document_chain = create_stuff_documents_chain(self.llm, self.prompt)
        rag_chain = create_retrieval_chain(retriever, document_chain)
        return rag_chain
    
    def generate_response(self, query: str, retriever):
        """Generate response using RAG"""
        rag_chain = self.create_rag_chain(retriever)
        response = rag_chain.invoke({"input": query})
        return response