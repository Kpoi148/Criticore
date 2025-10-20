/**
 * JavaScript functions to interact with your PDF Summarizer API
 * Add this to your existing JavaScript files
 */

class PDFSummarizerClient {
    constructor(baseURL = 'http://14.225.208.251:8000') {
        this.baseURL = baseURL;
    }

    /**
     * Upload and summarize a PDF file
     * @param {File} pdfFile - The PDF file from an input element
     * @returns {Promise} - Promise that resolves to the summary response
     */
    async summarizePDF(pdfFile) {
        // Validate file type
        if (!pdfFile.name.toLowerCase().endsWith('.pdf')) {
            throw new Error('Please select a PDF file');
        }

        // Create FormData for file upload
        const formData = new FormData();
        formData.append('file', pdfFile);

        try {
            const response = await fetch(`${this.baseURL}/summarize`, {
                method: 'POST',
                body: formData,
                // Don't set Content-Type header - browser will set it automatically with boundary
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Error uploading PDF:', error);
            throw error;
        }
    }

    /**
     * Check if the API is available
     */
    async checkHealth() {
        try {
            const response = await fetch(`${this.baseURL}/`);
            return response.ok;
        } catch (error) {
            return false;
        }
    }
}

/**
 * Example HTML form handler
 */
function setupPDFSummarizerForm() {
    const client = new PDFSummarizerClient();

    // Create HTML for PDF upload (add this to your HTML)
    const html = `
    <div class="pdf-summarizer">
        <h3>📄 PDF Summarizer</h3>
        <input type="file" id="pdfInput" accept=".pdf" />
        <button id="summarizeBtn" onclick="handlePDFUpload()">Summarize PDF</button>
        <div id="loadingDiv" style="display: none;">🔄 Processing PDF...</div>
        <div id="resultDiv"></div>
    </div>`;

    // Add to your page (example)
    // document.getElementById('your-container').innerHTML = html;

    // Handler function (make this global or adjust for your structure)
    window.handlePDFUpload = async function() {
        const fileInput = document.getElementById('pdfInput');
        const loadingDiv = document.getElementById('loadingDiv');
        const resultDiv = document.getElementById('resultDiv');
        const button = document.getElementById('summarizeBtn');

        if (!fileInput.files[0]) {
            alert('Please select a PDF file');
            return;
        }

        // Show loading
        loadingDiv.style.display = 'block';
        resultDiv.innerHTML = '';
        button.disabled = true;

        try {
            const result = await client.summarizePDF(fileInput.files[0]);

            if (result.success) {
                resultDiv.innerHTML = `
                    <div class="summary-result">
                        <h4>✅ Summary Generated!</h4>
                        <p><strong>File:</strong> ${result.file_name}</p>
                        <p><strong>Pages:</strong> ${result.page_count}</p>
                        <div class="summary-content">
                            <h5>Summary:</h5>
                            <pre>${result.summary}</pre>
                        </div>
                    </div>`;
            } else {
                resultDiv.innerHTML = `<div class="error">❌ Error: ${result.error}</div>`;
            }
        } catch (error) {
            resultDiv.innerHTML = `<div class="error">❌ Error: ${error.message}</div>`;
        } finally {
            loadingDiv.style.display = 'none';
            button.disabled = false;
        }
    };
}

/**
 * Simpler function for quick testing in browser console
 */
async function quickTestPDF() {
    // Create a file input for testing
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = '.pdf';
    
    input.onchange = async (e) => {
        const file = e.target.files[0];
        if (file) {
            console.log('Testing PDF upload...');
            const client = new PDFSummarizerClient();
            
            try {
                const result = await client.summarizePDF(file);
                console.log('✅ Success:', result);
            } catch (error) {
                console.error('❌ Error:', error);
            }
        }
    };
    
    input.click();
}

// Export for use in modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { PDFSummarizerClient, setupPDFSummarizerForm, quickTestPDF };
}