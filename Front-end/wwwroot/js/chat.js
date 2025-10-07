//async function callAiAPI(userInput, useRAG = false) {
//    const history = JSON.parse(localStorage.getItem('conversationHistory') || '[]');

//    // User/class từ localStorage như code gốc
//    const user = JSON.parse(localStorage.getItem('user') || '{}');
//    const senderId = user.sub || null;
//    const senderName = user.name || "Unknown";

//    const classes = JSON.parse(localStorage.getItem('classes') || '[]');
//    let currentClassId = localStorage.getItem('currentClassId') || (classes[0]?.class_id || null);

//    try {
//        const response = await fetch('/api/ai/ask', {  // .NET endpoint
//            method: 'POST',
//            headers: { 'Content-Type': 'application/json' },
//            body: JSON.stringify({ userInput, history, useRAG })  // Flag UseRAG nếu muốn RAG
//        });

//        const data = await response.json();

//        // Update history (code gốc dùng engagementHistory nếu cần)
//        history.push({ role: "user", content: userInput });
//        history.push({ role: "assistant", content: data.reply });
//        localStorage.setItem('conversationHistory', JSON.stringify(history));

//        return data.reply;
//    } catch (error) {
//        console.error('Error:', error);
//        return "Error calling AI.";
//    }
//}

//// Event listener như trước, gọi callAiAPI(prompt)