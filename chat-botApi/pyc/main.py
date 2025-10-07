from rag import RAG
import time

def main():
    start_time = time.time()
    
    # Initialize RAG system (should be fast now)
    rag = RAG()
    
    # Load documents
    pdf_path = r"documents\asv_sys.pdf"
    
    try:
        # Load documents (this is where the time will be spent)
        result = rag.load_documents(pdf_path)
        
        # Example query
        if rag.is_initialized:
            question = "What is the main topic of the document?"
            response = rag.query(question)
            
            if isinstance(response, dict):
                print(f"\nAnswer: {response.get('answer', 'No answer found')}")
                if 'context' in response:
                    print(f"Used {len(response['context'])} source documents")
            else:
                print(f"\nAnswer: {response}")
    
    except Exception as e:
        print(f"Error: {e}")
    
    total_time = time.time() - start_time
    print(f"\nTotal execution time: {total_time:.2f} seconds")

if __name__ == "__main__":
    main()