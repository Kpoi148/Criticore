// ================= Q&A Topic Detail ================
const googleUser = window.googleUser;
const params = new URLSearchParams(window.location.search);
const classId = params.get("class_id");
const topicId = params.get("topic_id");
let currentAnswerIndex = null;
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
            <span>${topic.created_by}</span>
            <span>&bull;</span>
            <span>${topic.created_at ? new Date(topic.created_at).toLocaleString() : ""}</span>
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
            <span class="text-gray-500 ml-2">(${a.voteCount || 0} đánh giá, trung bình ${rating.toFixed(1)})</span>
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
        const response = await fetch(`https://localhost:7134/api/TopicDetail/votes`, {
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
        const response = await fetch("https://localhost:7134/api/TopicDetail/answers", {
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
//// Hàm showToast để hiển thị thông báo
//function showToast(message, time = 3500, type = "warning") {
// const toast = document.getElementById("toast");
// toast.innerText = message;
// // Thay đổi màu sắc của toast tùy theo loại (warning, success, error)
// if (type === "success") {
// toast.style.backgroundColor = "#10b981"; // Màu xanh cho thành công
// } else if (type === "error") {
// toast.style.backgroundColor = "#ef4444"; // Màu đỏ cho lỗi
// } else {
// toast.style.backgroundColor = "#f59e0b"; // Màu vàng cho cảnh báo
// }
// toast.classList.remove("hidden");
// toast.classList.add("opacity-100");
// setTimeout(() => {
// toast.classList.add("hidden");
// toast.classList.remove("opacity-100");
// }, time);
//}
document.getElementById(
    "discussionLink"
).href = `topic-detail.html?class_id=${encodeURIComponent(
    classId
)}&topic_id=${encodeURIComponent(topicId)}`;
document.getElementById("thisHomeworkLink").href =
    `/Class/HomeworkList?class_id=${encodeURIComponent(classId)}&topic_id=${encodeURIComponent(topicId)}`;
document.getElementById("infoStudents").innerText = (
    cls.memberList || []
).length;
document.getElementById("infoAssignments").innerText = (
    topic.homeworks || []
).length;
document.getElementById("infoDiscussions").innerText = (
    cls.topics || []
).length;
document.getElementById("submitAnswer").addEventListener("click", sendAnswer);
// Thêm script SignalR nếu chưa có (trong HTML: <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>)
// Kết nối SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7134/topicHub") // URL API của TopicDetail (thay bằng production URL)
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
        <span class="text-gray-500 ml-2">(${voteCount} đánh giá, trung bình ${rating.toFixed(1)})</span>
        <span id="ratingLabel${index}" class="text-gray-500 ml-2 hidden"></span>
    `;
    attachStarHoverHandlersToAll(); // Re-attach hover nếu cần
}