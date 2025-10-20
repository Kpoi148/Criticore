// PDF Service for handling document uploads and queries

const API_CONFIG = {
    BASE_URL: 'http://14.225.208.251:8000',
    endpoints: {
        UPLOAD: '/upload',
        QUERY: '/query',
        CHAT: '/chat',
        DOCUMENTS_STATUS: '/documents/status',
        HEALTH: '/health'
    }
};

class PDFService {
    constructor(baseUrl = API_CONFIG.BASE_URL) {
        this.baseUrl = baseUrl;
    }
    
    async uploadPDF(pdfFile) {
        if (!pdfFile || !pdfFile.name.toLowerCase().endsWith('.pdf')) {
            throw new Error('Vui lòng chọn file PDF hợp lệ');
        }
        
        const formData = new FormData();
        formData.append('file', pdfFile);
        
        try {
            const res = await fetch(`${this.baseUrl}${API_CONFIG.endpoints.UPLOAD}`, {
                method: 'POST',
                body: formData
            });
            
            const data = await res.json();
            
            if (!res.ok) {
                throw new Error(data.detail || `API Error: ${res.status}`);
            }
            
            return data;
        } catch (error) {
            if (error.message.includes('Failed to fetch')) {
                throw new Error('Không thể kết nối đến server. Vui lòng kiểm tra kết nối hoặc server đã khởi động chưa.');
            }
            throw error;
        }
    }
    
    async queryDocuments(prompt, userId = null, k = 8) {
        try {
            const res = await fetch(`${this.baseUrl}${API_CONFIG.endpoints.QUERY}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    prompt,
                    user_profile: userId ? { user_id: userId } : undefined,
                    k
                })
            });
            
            const data = await res.json();
            
            if (!res.ok) {
                throw new Error(data.detail || `API Error: ${res.status}`);
            }
            
            return data;
        } catch (error) {
            if (error.message.includes('Failed to fetch')) {
                throw new Error('Không thể kết nối đến server. Vui lòng kiểm tra kết nối hoặc server đã khởi động chưa.');
            }
            throw error;
        }
    }
    
    async getDocumentsStatus() {
        try {
            const res = await fetch(`${this.baseUrl}${API_CONFIG.endpoints.DOCUMENTS_STATUS}`);
            if (!res.ok) {
                const data = await res.json();
                throw new Error(data.detail || `API Error: ${res.status}`);
            }
            return await res.json();
        } catch (error) {
            console.error("Error checking document status:", error);
            return { error: error.message };
        }
    }
    
    async checkHealth() {
        try {
            const res = await fetch(`${this.baseUrl}${API_CONFIG.endpoints.HEALTH}`);
            return res.ok;
        } catch (error) {
            console.error("Health check failed:", error);
            return false;
        }
    }
}

// Create a global instance
const pdfService = new PDFService();