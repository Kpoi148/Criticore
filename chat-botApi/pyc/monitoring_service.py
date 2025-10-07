import time
import threading
import psutil
import gc
from typing import Dict, List, Any
from datetime import datetime
import os

class MonitoringService:
    """Service for monitoring performance metrics"""
    
    def __init__(self):
        self.metrics: Dict[str, List[Dict[str, Any]]] = {
            "memory_usage": [],
            "document_processing": [],
            "query_performance": []
        }
        self._monitor_thread = None
        self._stop_event = threading.Event()
    
    def start_monitoring(self, interval: int = 60):
        """Start background monitoring thread"""
        def _monitor_loop():
            while not self._stop_event.is_set():
                self.collect_system_metrics()
                time.sleep(interval)
        
        self._monitor_thread = threading.Thread(target=_monitor_loop)
        self._monitor_thread.daemon = True
        self._monitor_thread.start()
        print(f"Performance monitoring started (interval: {interval}s)")
    
    def stop_monitoring(self):
        """Stop monitoring thread"""
        if self._monitor_thread:
            self._stop_event.set()
            self._monitor_thread.join(timeout=5)
            print("Performance monitoring stopped")
    
    def collect_system_metrics(self):
        """Collect system metrics"""
        process = psutil.Process(os.getpid())
        memory_info = process.memory_info()
        
        self.metrics["memory_usage"].append({
            "timestamp": datetime.now().isoformat(),
            "rss_mb": memory_info.rss / (1024 * 1024),
            "vms_mb": memory_info.vms / (1024 * 1024),
            "cpu_percent": process.cpu_percent(),
            "thread_count": len(process.threads())
        })
        
        # Keep only last 100 records
        if len(self.metrics["memory_usage"]) > 100:
            self.metrics["memory_usage"] = self.metrics["memory_usage"][-100:]
    
    def log_document_processing(self, filename: str, doc_size: int, processing_time: float, chunk_count: int):
        """Log document processing metrics"""
        self.metrics["document_processing"].append({
            "timestamp": datetime.now().isoformat(),
            "filename": filename,
            "size_kb": doc_size / 1024,
            "processing_time": processing_time,
            "chunk_count": chunk_count,
            "chunks_per_second": chunk_count / processing_time if processing_time > 0 else 0
        })
        
        # Keep only last 50 documents
        if len(self.metrics["document_processing"]) > 50:
            self.metrics["document_processing"] = self.metrics["document_processing"][-50:]
    
    def log_query_performance(self, query_length: int, retrieved_chunks: int, processing_time: float):
        """Log query performance metrics"""
        self.metrics["query_performance"].append({
            "timestamp": datetime.now().isoformat(),
            "query_length": query_length,
            "retrieved_chunks": retrieved_chunks,
            "processing_time": processing_time
        })
        
        # Keep only last 100 queries
        if len(self.metrics["query_performance"]) > 100:
            self.metrics["query_performance"] = self.metrics["query_performance"][-100:]
    
    def get_performance_report(self):
        """Get performance metrics report"""
        if not self.metrics["memory_usage"]:
            return {"status": "No metrics collected yet"}
            
        # Calculate averages and stats
        mem_metrics = self.metrics["memory_usage"]
        current_mem = mem_metrics[-1]["rss_mb"] if mem_metrics else 0
        avg_mem = sum(m["rss_mb"] for m in mem_metrics) / len(mem_metrics) if mem_metrics else 0
        
        doc_metrics = self.metrics["document_processing"]
        avg_doc_time = sum(d["processing_time"] for d in doc_metrics) / len(doc_metrics) if doc_metrics else 0
        
        query_metrics = self.metrics["query_performance"]
        avg_query_time = sum(q["processing_time"] for q in query_metrics) / len(query_metrics) if query_metrics else 0
        
        return {
            "current_memory_mb": round(current_mem, 2),
            "avg_memory_mb": round(avg_mem, 2),
            "avg_doc_processing_time": round(avg_doc_time, 2),
            "avg_query_time": round(avg_query_time, 2),
            "total_documents_processed": len(doc_metrics),
            "total_queries_processed": len(query_metrics),
            "last_collection": mem_metrics[-1]["timestamp"] if mem_metrics else None
        }
    
    def optimize_memory(self, force: bool = False):
        """Try to free memory if usage is high"""
        process = psutil.Process(os.getpid())
        memory_info = process.memory_info()
        memory_mb = memory_info.rss / (1024 * 1024)
        
        # If memory usage is over 1GB or force is True
        if memory_mb > 1024 or force:
            print(f"Memory usage high ({memory_mb:.2f} MB). Running garbage collection...")
            gc.collect()
            
            # Measure after cleanup
            memory_info = process.memory_info()
            new_memory_mb = memory_info.rss / (1024 * 1024)
            print(f"Memory after GC: {new_memory_mb:.2f} MB (freed {memory_mb - new_memory_mb:.2f} MB)")
            
            return {"before_mb": memory_mb, "after_mb": new_memory_mb}
        
        return {"current_mb": memory_mb, "status": "Memory usage acceptable"}

# Global instance for use across the application
monitor = MonitoringService()
