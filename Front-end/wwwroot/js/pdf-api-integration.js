// Integration with your existing API structure
// Add this to your existing JavaScript files

/**
 * Call PDF Summarizer API (similar to your callOpenAIAPI pattern)
 * @param {File} pdfFile - The PDF file to summarize
 * @returns {Promise<string>} - The summary text
 */
async function callPDFSummarizerAPI(pdfFile) {
    try {
        // Validate file
        if (!pdfFile || !pdfFile.name.toLowerCase().endsWith('.pdf')) {
            throw new Error('Vui lòng chọn file PDF hợp lệ');
        }

        // Create FormData for file upload
        const formData = new FormData();
        formData.append('file', pdfFile);

        // Call your Python FastAPI endpoint
        const res = await fetch('https://14.225.208.251:8000/summarize', {
            method: 'POST',
            body: formData,
        });

        const data = await res.json();

        if (!res.ok) {
            throw new Error(data.error || `Lỗi API: ${res.status}`);
        }

        if (!data.success) {
            throw new Error(data.error || 'Không thể tóm tắt PDF');
        }

        // Return formatted summary similar to your AI response format
        return `📄 **Tóm tắt PDF: ${data.file_name}**\n\n` +
            `📊 **Thông tin:**\n` +
            `• Số trang: ${data.page_count}\n\n` +
            `📝 **Nội dung tóm tắt:**\n${data.summary}`;

    } catch (error) {
        console.error('Lỗi gọi PDF Summarizer API:', error);
        throw new Error(`Lỗi xử lý PDF: ${error.message}`);
    }
}

/**
 * Enhanced version that integrates with your existing conversation history
 * @param {File} pdfFile - The PDF file
 * @returns {Promise<string>} - The summary with history integration
 */
async function callPDFSummarizerWithHistory(pdfFile) {
    try {
        const summary = await callPDFSummarizerAPI(pdfFile);

        // Get existing conversation history (following your pattern)
        const history = JSON.parse(localStorage.getItem('conversationHistory') || '[]');
        const user = JSON.parse(localStorage.getItem('user') || '{}');

        // Add PDF summary to conversation history
        const newEntry = {
            role: 'user',
            content: `Đã upload PDF: ${pdfFile.name}`,
            timestamp: new Date().toISOString(),
            type: 'pdf_upload'
        };

        const aiResponse = {
            role: 'assistant',
            content: summary,
            timestamp: new Date().toISOString(),
            type: 'pdf_summary'
        };

        history.push(newEntry, aiResponse);

        // Save updated history (following your existing pattern)
        localStorage.setItem('conversationHistory', JSON.stringify(history));

        return summary;

    } catch (error) {
        throw error;
    }
}

// Make functions available globally (following your pattern)
window.callPDFSummarizerAPI = callPDFSummarizerAPI;
window.callPDFSummarizerWithHistory = callPDFSummarizerWithHistory;

// Example usage in your existing chat interface:
/*
// Add this to your existing chat UI code
function handlePDFUpload() {
    const fileInput = document.getElementById('pdf-input');
    const file = fileInput.files[0];
    
    if (file) {
        // Show loading (use your existing UI patterns)
        showLoadingMessage('Đang xử lý PDF...');
        
        callPDFSummarizerWithHistory(file)
            .then(summary => {
                // Display in your existing chat interface
                displayAIResponse(summary);
                hideLoading();
            })
            .catch(error => {
                displayErrorMessage(error.message);
                hideLoading();
            });
    }
}
*/