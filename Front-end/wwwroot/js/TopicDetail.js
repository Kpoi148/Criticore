// ================= Q&A Topic Detail ================
const googleUser = window.googleUser;
const params = new URLSearchParams(window.location.search);
const classId = params.get("class_id");
const topicId = params.get("topic_id");
let currentAnswerIndex = null;
let lastAIReply = ""; // Lưu phản hồi AI cuối cùng để kiểm tra tương đồng
// Gần đầu file, sau Notyf
// Line ~9: Init chỉ nếu class tồn tại
let pdfService;
if (typeof PDFSummarizerClient !== 'undefined') {
    pdfService = new PDFSummarizerClient('http://localhost:8000');  // URL backend Python
} else {
    console.error('PDFSummarizerClient not loaded! Check script include.');
}

marked.setOptions({
    breaks: true, // xuống dòng đúng
    gfm: true,    // hỗ trợ emoji, checklist, bảng, code block
});

const notyf = new Notyf({
    duration: 4000, // Thời gian hiển thị (ms)
    ripple: true, // Effect ripple hiện đại
    position: { x: 'right', y: 'top' }, // Vị trí top-right
    dismissible: true, // Cho phép click để đóng
    types: [
        { type: 'success', background: '#10b981', icon: false }, // Màu xanh Tailwind green-500
        { type: 'error', background: '#ef4444', icon: false }, // Màu đỏ Tailwind red-500
        { type: 'warning', background: '#f59e0b', icon: false } // Màu vàng Tailwind yellow-500
    ]
});
// Tên đánh giá tương ứng với mỗi sao
const starLabels = {
    1: "Rất tệ",
    2: "Tệ",
    3: "Bình thường",
    4: "Tốt",
    5: "Rất tốt",
};
const cls = window.cls;
const topic = window.topic;
// Hiển thị header và prompt
function renderTopicInfo() {
    if (!topic) {
        console.error('Topic không tồn tại hoặc chưa load.');
        document.getElementById("topicInfo").innerHTML = '<p>Lỗi: Không tìm thấy chủ đề.</p>';
        return;
    }
    document.getElementById("topicInfo").innerHTML = `
        <a href="/Class/ClassDetail?id=${encodeURIComponent(classId)}" class="text-blue-600 text-sm">&larr; Back to class</a>
        <h1 class="text-3xl font-bold mt-2 mb-2">${topic.title}</h1>
        <div class="flex items-center gap-2 text-sm text-gray-600">
            <span class="bg-gray-200 px-2 py-0.5 rounded-full text-xs font-semibold">${topic.role || "Teacher"}</span>
            <span>${topic.createdByName}</span>
            <span>&bull;</span>
            <span>${topic.createdAt ? new Date(topic.createdAt).toLocaleString() : ""}</span>
        </div>
    `;
    document.getElementById("promptBlock").innerHTML = `
      <p class="text-gray-800 mb-4">${topic.description}</p>
      <div class="flex items-center gap-6 text-sm text-gray-500">
        <div>💬 <span id="replyCount">${topic.replies || 0}</span> replies</div>
      </div>
    `;
}
renderTopicInfo();
// Fetch và render answers từ API
async function fetchAndRenderAnswers() {
    try {
        const response = await fetch(`https://localhost:7134/api/TopicDetail/topics/${topicId}/answers`);
        //const response = await fetch(`https://topicdetail.criticore.edu.vn:8009/api/TopicDetail/topics/${topicId}/answers`);
        if (!response.ok) throw new Error('Lỗi fetch answers');
        const answers = await response.json();
        topic.answers = answers; // Lưu tạm vào topic để dễ xử lý
        renderAnswers(answers);
        updateReplyCount(answers.length); // Cập nhật tổng replies
    } catch (error) {
        console.error(error);
        notyf.error("Lỗi tải câu trả lời từ server!");
    }
}
// Cập nhật tổng replies ở prompt
function updateReplyCount(count) {
    document.getElementById("replyCount").innerText = count;
}
// Render answers với sao (cập nhật để hỗ trợ fractional rating và voteCount)
function renderAnswers(answers) {
    const box = document.getElementById("answersList");
    if (!answers || !answers.length) {
        box.innerHTML = `<div class="text-gray-500">No answers yet.</div>`;
        return;
    }
    box.innerHTML = answers
        .map((a, index) => {
            console.log('Avatar URL for answer ' + a.answerId + ':', a.avatarUrl);
            let avatarHtml = (a.avatarUrl && typeof a.avatarUrl === 'string' && a.avatarUrl.startsWith("<span"))
                ? a.avatarUrl
                : (a.avatarUrl ? `<img src="${a.avatarUrl}" alt="avatar" class="w-9 h-9 rounded-full object-cover"/>` : '<span class="w-9 h-9 rounded-full bg-gray-300 flex items-center justify-center text-white">?</span>'); // Fallback avatar mặc định
            const rating = a.rating || 0;
            const fullStars = Math.floor(rating);
            const hasHalf = rating % 1 >= 0.5;
            const starsHtml = [1, 2, 3, 4, 5].map(star => {
                let className = 'star';
                if (star <= fullStars) className += ' selected';
                else if (star === fullStars + 1 && hasHalf) className += ' half';
                return `<span class="${className}" onclick="rateAnswer(${star}, ${index}); event.stopPropagation();" onmouseover="showRatingLabel(${star}, ${index})" onmouseout="hideRatingLabel(${index})">★</span>`;
            }).join("");
            return `
      <div onclick="openAnswerDetail(${index})"
           class="bg-white p-4 rounded-xl shadow hover:shadow-xl transition border cursor-pointer transform hover:scale-[1.02]">
        <div class="flex items-center gap-3 mb-3">
          ${avatarHtml}
          <div>
            <div class="font-semibold">${a.createdBy || 'Unknown User'}</div>
            <div class="text-xs text-gray-500">${new Date(a.createdAt).toLocaleString('vi-VN', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit' }) || ''}</div>
          </div>
        </div>
        <div class="text-sm text-gray-700 max-h-32 overflow-y-auto whitespace-pre-line">
          ${a.content}
        </div>
        <div class="mt-3 text-sm text-gray-500">
          <div id="likeCount">
            ${starsHtml}
            <span class="text-gray-500 ml-2">(${a.voteCount || 0} ratings, average ${rating.toFixed(1)})</span>
            <span id="ratingLabel${index}" class="text-gray-500 ml-2 hidden"></span>
          </div>
        </div>
      </div>`;
        })
        .join("");
    attachStarHoverHandlersToAll();
}
// Gọi fetch khi load
fetchAndRenderAnswers();
renderAnswers();
attachStarHoverHandlersToAll();
// Hàm xử lý khi người dùng chọn sao
// Rate answer (POST create/update vote)
async function rateAnswer(rating, index = null) {
    let answer;
    if (index !== null) {
        answer = topic.answers[index];
    } else if (typeof currentAnswerIndex === "number") {
        answer = topic.answers[currentAnswerIndex];
    }
    if (!answer) return;
    // Lấy token từ sessionStorage
    const token = sessionStorage.getItem('authToken');
    if (!token) {
        notyf.error("Vui lòng đăng nhập trước!");
        return;
    }
    // Parse payload từ token để lấy UserId
    const payload = token.split('.')[1];
    const decodedPayload = JSON.parse(atob(payload));
    const userId = parseInt(decodedPayload.UserId);
    const voteDto = {
        answerId: answer.answerId,
        userId: userId,
        voteType: "Rating", // Giả sử VoteType là "Rating" cho đánh giá sao
        amount: rating
    };
    try {
        const response = await fetch(`https://localhost:7134/api/TopicDetail/votes`
        //const response = await fetch(`https://topicdetail.criticore.edu.vn:8009/api/TopicDetail/votes`
            , {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}` // Thêm header auth nếu backend yêu cầu
            },
            body: JSON.stringify(voteDto),
        });
        if (!response.ok) throw new Error('Lỗi vote');
        notyf.success("Đánh giá đã được cập nhật!");
        // Temp update: Giả sử rating mới (cho user thấy thay đổi ngay, SignalR sẽ override average thực)
        topic.answers[index].rating = rating; // Placeholder (chỉ đúng nếu vote đầu, nhưng tốt cho UX)
        updateAnswerStars(index); // Update list ngay
        if (index === currentAnswerIndex) renderStars(rating); // Update modal
        // Optional fallback nếu SignalR delay > 2s: Re-fetch single answer
        setTimeout(async () => {
            const resp = await fetch(`https://localhost:7134/api/TopicDetail/answers/${answer.answerId}`);
            //const resp = await fetch(`https://topicdetail.criticore.edu.vn:8009/api/TopicDetail/answers/${answer.answerId}`);
            const updatedAnswer = await resp.json();
            topic.answers[index].rating = updatedAnswer.rating;
            topic.answers[index].voteCount = updatedAnswer.voteCount;
            updateAnswerStars(index);
            if (index === currentAnswerIndex) renderStars(updatedAnswer.rating);
        }, 2000); // Fallback nếu SignalR không trigger
    } catch (error) {
        console.error(error);
        notyf.error("Lỗi cập nhật đánh giá!");
    }
}
// Hàm gán hiệu ứng hover cho tất cả sao trên danh sách
function attachStarHoverHandlersToAll() {
    document.querySelectorAll("#answersList .star").forEach((star) => {
        const parent = star.parentElement;
        const allStars = Array.from(parent.querySelectorAll(".star"));
        const idx = allStars.indexOf(star);
        let rating = allStars.filter((s) =>
            s.classList.contains("selected")
        ).length;
        star.onmouseover = function () {
            allStars.forEach((s, i) => {
                if (i <= idx) s.classList.add("selected");
                else s.classList.remove("selected");
            });
        };
        star.onmouseout = function () {
            allStars.forEach((s, i) => {
                if (i < rating) s.classList.add("selected");
                else s.classList.remove("selected");
            });
        };
    });
}
// Hàm hiển thị các sao đã chọn (cập nhật để hỗ trợ fractional)
function renderStars(rating = 0) {
    const stars = document.querySelectorAll("#answerModal #likeCount .star");
    const fullStars = Math.floor(rating);
    const hasHalf = rating % 1 >= 0.5;
    stars.forEach((star, index) => {
        star.classList.remove("selected", "half");
        if (index < fullStars) {
            star.classList.add("selected");
        } else if (index === fullStars && hasHalf) {
            star.classList.add("half");
        }
        star.onmouseover = function () {
            stars.forEach((s, i) => {
                s.classList.remove("selected", "half");
                if (i <= index) s.classList.add("selected");
            });
            showRatingLabel(index + 1, null);
        };
        star.onmouseout = function () {
            stars.forEach((s, i) => {
                s.classList.remove("selected", "half");
                if (i < fullStars) s.classList.add("selected");
                else if (i === fullStars && hasHalf) s.classList.add("half");
            });
            hideRatingLabel(null);
        };
        star.onclick = function (e) {
            e.stopPropagation();
            rateAnswer(index + 1, null);
        };
    });
}
// Hàm hiển thị tên sao khi hover
function showRatingLabel(rating, index = null) {
    let label;
    if (index !== null && document.getElementById(`ratingLabel${index}`)) {
        label = document.getElementById(`ratingLabel${index}`);
    } else {
        label = document.getElementById("ratingLabel");
    }
    if (!label) return;
    label.innerText = starLabels[rating] || "";
    label.classList.remove("hidden");
}
// Hàm ẩn tên sao khi không hover
function hideRatingLabel(index = null) {
    let label;
    if (index !== null && document.getElementById(`ratingLabel${index}`)) {
        label = document.getElementById(`ratingLabel${index}`);
    } else {
        label = document.getElementById("ratingLabel");
    }
    if (!label) return;
    label.classList.add("hidden");
}
// ================= Câu trả lời chi tiết ================
// xem chi tiết câu trả lời
function openAnswerDetail(index) {
    currentAnswerIndex = index;
    const ans = topic.answers[index];
    // avatar: span hay img
    const avatarHtml = ans.picture.startsWith("<span")
        ? ans.picture
        : `<img src="${ans.picture}" alt="avatar" class="w-10 h-10 rounded-full object-cover"/>`;
    // Mở modal chi tiết câu trả lời
    document.getElementById("answerModal").classList.remove("hidden");
    // ✅ Cập nhật avatar (innerHTML thay vì .src)
    document.getElementById("answerAvatar").innerHTML = avatarHtml;
    // Cập nhật thông tin câu trả lời
    document.getElementById("answerAuthor").innerText = ans.created_by || "Lỗi hiển thị";
    document.getElementById("answerText").innerText = ans.content;
    // Hiển thị đánh giá sao
    renderStars(ans.rating || 0);
    // Cập nhật số lượng phản hồi
    document.getElementById("replyCount").innerText = `${ans.replies?.length || 0} replies`;
    // Hiển thị các phản hồi
    renderReplies();
}
// đóng chi tiết câu trả lời
function closeAnswerDetail() {
    document.getElementById("answerModal").classList.add("hidden");
    currentAnswerIndex = null;
}
function sendReply() {
    const input = document.getElementById("replyInput");
    const replyText = input.value.trim();
    if (!replyText) return notyf.error("Nhập phản hồi trước khi gửi!");
    const answer = topic.answers[currentAnswerIndex];
    answer.replies = answer.replies || [];
    answer.replies.push({
        by: googleUser.name || "Bạn",
        text: replyText,
        at: new Date().toLocaleString(),
        picture: googleUser.picture || "https://via.placeholder.com/40", // <== thêm dòng này
    });
    // Lưu lại
    localStorage.setItem("classes", JSON.stringify(classes));
    input.value = "";
    renderReplies();
}
function renderReplies() {
    const list = document.getElementById("replyList");
    const answer = topic.answers[currentAnswerIndex];
    const replies = answer.replies || [];
    if (!replies.length) {
        list.innerHTML = `<div class="text-gray-400 italic">Chưa có phản hồi nào.</div>`;
        return;
    }
    list.innerHTML = replies
        .map(
            (r) => `
  <div class="flex items-start gap-3">
<img src="${r.picture || "https://via.placeholder.com/40"}" alt="avatar"
     class="w-9 h-9 rounded-full object-cover shrink-0" />
    <div class="flex-1 bg-gray-100 rounded-lg px-4 py-3">
      <div class="flex justify-between items-center mb-1">
        <div class="font-medium text-sm text-gray-900">${r.by}</div>
        <div class="text-xs text-gray-500">${r.at}</div>
      </div>
      <div class="text-sm text-gray-800">${r.text}</div>
      <div class="mt-2 text-sm text-gray-500">♡ 0</div>
    </div>
  </div>
`
        )
        .join("");
}
// Khi người dùng gửi trả lời (POST qua API)
async function sendAnswer() {
    const ta = document.getElementById("answerContent");
    const content = ta.value.trim();
    if (!content) return notyf.error("Nhập nội dung trả lời");
    // Chuẩn hóa nội dung và gợi ý AI
    let normalizedInput = normalizeText(content);
    let normalizedAI = normalizeText(lastAIReply);
    let sim = diceCoefficient(normalizedInput, normalizedAI);
    // Nếu độ tương đồng lớn hơn 50%, cảnh báo và không gửi
    if (sim > 0.5) {
        notyf.warning(
            "⚠️ Câu trả lời của bạn quá giống gợi ý AI (" +
            Math.round(sim * 100) +
            "%)! Hãy tự diễn đạt lại."
        );
        return; // Chặn không cho gửi
    }
    // Lấy token từ localStorage
    const token = sessionStorage.getItem('authToken');
    if (!token) {
        notyf.error("Vui lòng đăng nhập trước!");
        return;
    }
    // Parse payload từ token để lấy UserId (phần giữa các dấu chấm)
    const payload = token.split('.')[1];
    const decodedPayload = JSON.parse(atob(payload)); // atob() decode base64
    const userId = parseInt(decodedPayload.UserId); // Lấy UserId (là "1" trong token mẫu)
    // Kiểm tra và fallback từ googleUser (nếu có)
    const userName = (window.googleUser && window.googleUser.name) ? window.googleUser.name : "Bạn";
    const userPicture = (window.googleUser && window.googleUser.picture) ? window.googleUser.picture : "https://via.placeholder.com/40";
    // Body POST khớp với CreateAnswerDto (thêm userId)
    const newAnswer = {
        topicId: parseInt(topicId),
        userId: userId, // Thêm UserId từ token
        content: content,
        // Nếu backend hỗ trợ, thêm createdBy và picture (xem phần 4)
        // createdBy: userName,
        // picture: userPicture,
    };
    try {
        const response = await fetch("https://localhost:7134/api/TopicDetail/answers"
        //const response = await fetch("https://topicdetail.criticore.edu.vn:8009/api/TopicDetail/answers"
            , {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}` // Thêm header auth nếu backend yêu cầu
            },
            body: JSON.stringify(newAnswer),
        });
        if (!response.ok) throw new Error('Lỗi gửi answer');
        notyf.success("Câu trả lời đã được gửi thành công!");
        ta.value = "";
        ta.style.height = "auto";
        fetchAndRenderAnswers(); // Refresh list
    } catch (error) {
        console.error(error);
        notyf.error("Lỗi gửi câu trả lời!");
    }
}
// Tự động giãn textarea trả lời
const answerContent = document.getElementById("answerContent");
answerContent.addEventListener("input", function () {
    this.style.height = "auto";
    this.style.height = this.scrollHeight + "px";
});
answerContent.addEventListener("keydown", function (e) {
    // Shift+Enter xuống dòng, Enter gửi
    if (e.key === "Enter" && !e.shiftKey) {
        sendAnswer();
        e.preventDefault();
    }
});
const textarea = document.getElementById("answerContent");
const answerBar = document.getElementById("answerBar");
textarea.addEventListener("focus", () => {
    answerBar.classList.add("fullscreen-answer");
});
textarea.addEventListener("blur", () => {
    // Delay để tránh mất khi click nút Gửi
    setTimeout(() => {
        answerBar.classList.remove("fullscreen-answer");
    }, 200);
});
document.getElementById(
    "discussionLink"
).href = `topic-detail.html?class_id=${encodeURIComponent(
    classId
)}&topic_id=${encodeURIComponent(topicId)}`;
document.getElementById("thisHomeworkLink").href =
    `/Class/HomeworkList?class_id=${encodeURIComponent(classId)}&topic_id=${encodeURIComponent(topicId)}`;
document.getElementById("submitAnswer").addEventListener("click", sendAnswer);
// Thêm script SignalR nếu chưa có (trong HTML: <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>)
// Kết nối SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7134/topicHub") // URL API của TopicDetail (thay bằng production URL)
    //.withUrl("https://topicdetail.criticore.edu.vn:8009/topicHub") // URL API của TopicDetail (thay bằng production URL)
    .withAutomaticReconnect() // Tự reconnect
    .build();
connection.start()
    .then(() => console.log("Kết nối SignalR thành công!"))
    .catch(err => console.error("Lỗi kết nối SignalR:", err));
// Listen events từ server
connection.on("NewAnswer", (newAnswer) => {
    // Append answer mới vào list mà không reload
    topic.answers.push(newAnswer);
    renderAnswers(topic.answers);
    updateReplyCount(topic.answers.length);
    notyf.success("Có câu trả lời mới!");
});
connection.on("UpdatedAnswer", (updatedData) => {
    // Tìm và update answer trong list
    const index = topic.answers.findIndex(a => a.answerId === updatedData.AnswerId);
    if (index !== -1) {
        topic.answers[index].content = updatedData.UpdatedContent; // Update field cần
        renderAnswers(topic.answers);
        if (currentAnswerIndex === index) {
            document.getElementById("answerText").innerText = updatedData.UpdatedContent; // Update modal nếu mở
        }
        notyf.success("Câu trả lời đã được cập nhật!");
    }
});
connection.on("DeletedAnswer", (deletedId) => {
    // Xóa answer khỏi list
    topic.answers = topic.answers.filter(a => a.answerId !== deletedId);
    renderAnswers(topic.answers);
    updateReplyCount(topic.answers.length);
    if (currentAnswerIndex !== null && topic.answers[currentAnswerIndex]?.answerId === deletedId) {
        closeAnswerDetail(); // Đóng modal nếu đang xem answer bị xóa
    }
    notyf.warning("Câu trả lời đã bị xóa!");
});
// Update listeners SignalR với debug
connection.on("CreatedVote", (data) => {
    console.log("CreatedVote received:", data); // Debug
    const index = topic.answers.findIndex(a => a.answerId === data.AnswerId);
    if (index !== -1) {
        topic.answers[index].rating = data.NewAverage;
        topic.answers[index].voteCount = data.NewVoteCount;
        updateAnswerStars(index);
        notyf.success("Có lượt đánh giá mới!");
    }
});
connection.on("UpdatedVote", (data) => {
    console.log("UpdatedVote received:", data); // Debug
    const index = topic.answers.findIndex(a => a.answerId === data.AnswerId);
    if (index !== -1) {
        topic.answers[index].rating = data.NewAverage;
        topic.answers[index].voteCount = data.NewVoteCount;
        updateAnswerStars(index);
        if (currentAnswerIndex === index) {
            renderStars(data.NewAverage);
        }
        notyf.success("Lượt đánh giá đã được cập nhật!");
    }
});
// Lắng nghe vote bị xóa
connection.on("DeletedVote", (data) => {
    const index = topic.answers.findIndex(a => a.answerId === data.AnswerId);
    if (index !== -1) {
        topic.answers[index].rating = data.NewAverage;
        topic.answers[index].voteCount = data.NewVoteCount;
        updateAnswerStars(index);
        if (currentAnswerIndex === index) {
            renderStars(data.NewAverage); // Update modal nếu đang mở
        }
        notyf.warning("Một lượt đánh giá đã bị xóa!");
    }
});
// Hàm mới: Update chỉ phần stars của 1 answer cụ thể (selective DOM update)
function updateAnswerStars(index) {
    const answerElements = document.querySelectorAll("#answersList > div");
    const answerElement = answerElements[index];
    if (!answerElement) return;
    const likeCount = answerElement.querySelector("#likeCount");
    const rating = topic.answers[index].rating || 0;
    const voteCount = topic.answers[index].voteCount || 0;
    const fullStars = Math.floor(rating);
    const hasHalf = rating % 1 >= 0.5;
    const starsHtml = [1, 2, 3, 4, 5].map(star => {
        let className = 'star';
        if (star <= fullStars) className += ' selected';
        else if (star === fullStars + 1 && hasHalf) className += ' half';
        return `<span class="${className}" onclick="rateAnswer(${star}, ${index}); event.stopPropagation();" onmouseover="showRatingLabel(${star}, ${index})" onmouseout="hideRatingLabel(${index})">★</span>`;
    }).join("");
    likeCount.innerHTML = `
        ${starsHtml}
        <span class="text-gray-500 ml-2">(${voteCount} ratings, average  ${rating.toFixed(1)})</span>
        <span id="ratingLabel${index}" class="text-gray-500 ml-2 hidden"></span>
    `;
    attachStarHoverHandlersToAll(); // Re-attach hover nếu cần
}
// ================= Tính năng chat với AI ================
async function callAiAPI(userInput, useRAG = false) {
    let history = JSON.parse(localStorage.getItem('conversationHistory') || '[]');
    // Thêm system prompt nếu chưa có (để enforce ở frontend)
    const systemPrompt = {
        role: "system",
        content: "You are an AI assistant who uses the Socratic method. Reply briefly (1–2 sentences) first, then ask one thoughtful question. Use rich Markdown (emoji, lists, **bold**, titles, +, and bullet points)."
    };
    // Kiểm tra và thêm nếu thiếu
    if (!history.some(msg => msg.role === 'system')) {
        history.unshift(systemPrompt);  // Thêm ở đầu history
    }
    try {
        console.log('History sent to API:', history);  // Debug để kiểm tra
        const response = await fetch('https://localhost:7209/api/ai/ask', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userInput, history, useRAG })
        });
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP Error! Status: ${response.status}, Message: ${errorText || 'No details'}`);
        }
        const data = await response.json();
        history.push({ role: "user", content: userInput });
        history.push({ role: "assistant", content: data.reply });
        localStorage.setItem('conversationHistory', JSON.stringify(history));
        return data.reply;
    } catch (error) {
        console.error('AI API Error:', error);
        return `Error calling AI: ${error.message}`;
    }
}
// Cập nhật sendMessage để append bubble user và AI
async function sendMessage() {
    const input = document.getElementById("userInput");
    const messages = document.getElementById("messages");
    const fileInput = document.getElementById("pdfInput");
    const textInput = input.value.trim();
    const pdfFile = fileInput?.files[0];

    // Clear text ngay khi gửi
    input.value = "";
    input.style.height = "auto";
    if (!textInput && !pdfFile) return;

    let loadingId = `loading-${Date.now()}`;

    // Show user message bubble (right-aligned)
    if (textInput) {
        messages.innerHTML += `
        <div class="flex justify-end mb-4">
            <div class="bg-blue-100 px-4 py-2 rounded-lg shadow-sm max-w-[80%] text-sm text-gray-800">
                ${escapeHtml(textInput)}
            </div>
        </div>`;
    }

    // Handle PDF upload
    if (pdfFile) {
        messages.innerHTML += `
        <div class="flex justify-end mb-4">
            <div class="bg-blue-100 px-4 py-2 rounded-lg shadow-sm max-w-[80%] text-sm text-gray-800">
                📄 Uploaded: ${escapeHtml(pdfFile.name)}
            </div>
        </div>`;
    }

    messages.innerHTML += `<div id="${loadingId}" class="flex items-start mb-4"><div class="bg-gray-200 px-4 py-2 rounded-lg text-sm">AI đang xử lý...</div></div>`;
    messages.scrollTop = messages.scrollHeight;

    try {
        let aiReply;
        let useRAG = !!pdfFile;

        if (pdfFile) {
            // Upload và query PDF (giữ nguyên logic cũ)
            const uploadResult = await pdfService.uploadPDF(pdfFile);
            if (!uploadResult.message.includes("successfully")) {
                throw new Error(`Upload failed: ${uploadResult.message}`);
            }
            const queryPrompt = textInput || `Tóm tắt những điểm chính trong file PDF "${pdfFile.name}"`;
            const queryResult = await pdfService.queryDocuments(queryPrompt, googleUser.sub, 8);
            aiReply = queryResult.response;
        } else {
            aiReply = await callAiAPI(textInput, false);
        }

        // Remove loading và display AI bubble
        document.getElementById(loadingId).remove();
        lastAIReply = aiReply;
        messages.innerHTML += `
        <div class="flex items-start mb-4">
           <div class="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center mr-2 overflow-hidden">
          <img src="/images/bot-assistant.gif" alt="AI" class="w-6 h-6 rounded-full">
        </div>

          <div class="prose prose-slate max-w-none bg-white px-4 py-3 rounded-2xl shadow-sm text-gray-800 whitespace-pre-line markdown-content">
            ${marked.parse(aiReply)}
            <div class="absolute top-1/2 -right-2 w-0 h-0 border-t-8 border-t-transparent border-l-8 border-l-white border-b-8 border-b-transparent transform -translate-y-1/2"></div>
          </div>
        </div>`;

    } catch (error) {
        document.getElementById(loadingId).remove();
        messages.innerHTML += `
        <div class="flex items-start mb-4">
            <div class="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center mr-2">
                <img src="~/images/bot-assistant.gif" alt="AI" class="w-6 h-6 rounded-full">
            </div>
            <div class="bg-white px-4 py-2 rounded-lg shadow-sm max-w-[80%] relative text-sm text-red-600">
                Lỗi: ${escapeHtml(error.message || 'Không thể xử lý yêu cầu')}
                <div class="absolute top-1/2 -right-2 w-0 h-0 border-t-8 border-t-transparent border-l-8 border-l-white border-b-8 border-b-transparent transform -translate-y-1/2"></div>
            </div>
        </div>`;
        console.error("Chat Error:", error);
    }

    // Clear inputs
    input.value = "";
    if (fileInput) fileInput.value = "";
    input.style.height = "auto";
    messages.scrollTop = messages.scrollHeight;
}
// Toggle chat với animation (đã có class chat-popup)
function toggleChat(show) {
    const popup = document.getElementById("chatPopup");
    if (show) {
        popup.classList.remove("hidden");
        popup.classList.add("chat-popup"); // Trigger animation
        setTimeout(() => document.getElementById("userInput").focus(), 100);
    } else {
        popup.classList.add("hidden");
        popup.classList.remove("chat-popup");
    }
}
function escapeHtml(str) {
    return str.replace(/[&<>"']/g, function (m) {
        return {
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#39;",
        }[m];
    });
}
// Event listeners cho chat AI
document.getElementById("openChat").addEventListener("click", () => toggleChat(true));
document.getElementById("closeChat").addEventListener("click", () => toggleChat(false));
document.getElementById("sendBtn").addEventListener("click", sendMessage);
const userInput = document.getElementById("userInput");
userInput.addEventListener("input", function () {
    this.style.height = "auto";
    this.style.height = this.scrollHeight + "px";
});
userInput.addEventListener("keydown", function (e) {
    if (e.key === "Enter" && !e.shiftKey) {
        sendMessage();
        e.preventDefault();
    }
});

// Function cho clear history
document.getElementById("clearChatHistory").addEventListener("click", function () {
    if (confirm("Bạn có chắc muốn xóa lịch sử chat?")) {
        localStorage.removeItem('conversationHistory');
        document.getElementById("messages").innerHTML = `
            <div class="text-center text-xs text-gray-500 mb-2">Hôm nay, ${new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}</div>
            <div class="flex items-start mb-4">
                <div class="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center mr-2">
                    <img src="~/images/bot-assistant.gif" alt="AI" class="w-6 h-6 rounded-full">
                </div>
                <div class="bg-white px-4 py-2 rounded-lg shadow-sm max-w-[80%] relative">
                    <p class="text-sm text-gray-800">Xin chào! Tôi có thể giúp gì cho bạn hôm nay?</p>
                    <div class="absolute top-1/2 -right-2 w-0 h-0 border-t-8 border-t-transparent border-l-8 border-l-white border-b-8 border-b-transparent transform -translate-y-1/2"></div>
                </div>
            </div>
            <div class="grid grid-cols-2 gap-2 mb-4">
                <button class="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm hover:bg-purple-200 transition">Tóm tắt PDF</button>
                <button class="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm hover:bg-purple-200 transition">Hỏi về chủ đề</button>
                <button class="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm hover:bg-purple-200 transition">Gợi ý bài tập</button>
                <button class="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm hover:bg-purple-200 transition">Tìm tài liệu</button>
            </div>
        `; // Reset UI với initial message
        notyf.success("Lịch sử chat đã được xóa!");
    }
});

