// ================= Q&A Topic Detail ================
const googleUser = window.googleUser;
const params = new URLSearchParams(window.location.search);
const classId = params.get("class_id");
const topicId = params.get("topic_id");

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
document.getElementById("topicInfo").innerHTML = `
    <a href="class-detail.html?id=${encodeURIComponent(classId)}" class="text-blue-600 text-sm">&larr; Back to class</a>
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
    <div>❤️ ${topic.likes || 0}</div>
    <div>💬 ${topic.replies || (topic.answers || []).length} replies</div>
  </div>
`;

// Cập nhật lại renderAnswers() để hiển thị sao khi trả lời được hiển thị
function renderAnswers() {
    const box = document.getElementById("answersList");
    if (!topic.answers || !topic.answers.length) {
        box.innerHTML = `<div class="text-gray-500">No answers yet.</div>`;
        return;
    }

    box.innerHTML = topic.answers
        .map((a, index) => {
            // Xử lý avatar: span hoặc ảnh
            let avatarHtml = a.picture.startsWith("<span")
                ? a.picture
                : `<img src="${a.picture}" alt="avatar" class="w-9 h-9 rounded-full object-cover"/>`;

            return `
      <div onclick="openAnswerDetail(${index})"
           class="bg-white p-4 rounded-xl shadow hover:shadow-xl transition border cursor-pointer transform hover:scale-[1.02]">
        <div class="flex items-center gap-3 mb-3">
          ${avatarHtml}
          <div>
            <div class="font-semibold">${a.created_by}</div>
            <div class="text-xs text-gray-500">${a.created_at}</div>
          </div>
        </div>
        <div class="text-sm text-gray-700 max-h-32 overflow-y-auto whitespace-pre-line">
          ${a.content}
        </div>
        <div class="mt-3 text-sm text-gray-500">
          <div id="likeCount">
            ${[1, 2, 3, 4, 5]
                    .map(
                        (star) =>
                            `<span class="star ${star <= (a.rating || 0) ? "selected" : ""
                            }" onclick="rateAnswer(${star}, ${index}); event.stopPropagation();" onmouseover="showRatingLabel(${star}, ${index})" onmouseout="hideRatingLabel(${index})">
              ★
            </span>`
                    )
                    .join("")}
            <span id="ratingLabel${index}" class="text-gray-500 ml-2 hidden"></span>
          </div>
        </div>
      </div>`;
        })
        .join("");
}

renderAnswers();
attachStarHoverHandlersToAll();

// Hàm xử lý khi người dùng chọn sao
function rateAnswer(rating, index = null) {
    let answer;
    if (index !== null) {
        answer = topic.answers[index];
        answer.rating = rating;
        localStorage.setItem("classes", JSON.stringify(classes));
        renderAnswers();
        // Nếu modal đang mở đúng answer này thì cập nhật sao luôn cho modal
        if (
            document.getElementById("answerModal") &&
            !document.getElementById("answerModal").classList.contains("hidden") &&
            typeof currentAnswerIndex === "number" &&
            currentAnswerIndex === index
        ) {
            renderStars(answer.rating);
        }
    } else if (typeof currentAnswerIndex === "number") {
        answer = topic.answers[currentAnswerIndex];
        answer.rating = rating;
        localStorage.setItem("classes", JSON.stringify(classes));
        renderStars(answer.rating);
        renderAnswers();
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

// Hàm hiển thị các sao đã chọn
function renderStars(rating = 0) {
    const stars = document.querySelectorAll("#answerModal #likeCount .star");
    stars.forEach((star, index) => {
        if (index < rating) {
            star.classList.add("selected");
        } else {
            star.classList.remove("selected");
        }
        star.onmouseover = function () {
            stars.forEach((s, i) => {
                if (i <= index) s.classList.add("selected");
                else s.classList.remove("selected");
            });
            showRatingLabel(index + 1, null);
        };
        star.onmouseout = function () {
            stars.forEach((s, i) => {
                if (i < rating) s.classList.add("selected");
                else s.classList.remove("selected");
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
// =================    Câu trả lời chi tiết ================
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
    if (!replyText) return alert("Nhập phản hồi trước khi gửi!");

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

// Khi người dùng gửi trả lời
function sendAnswer() {
    const ta = document.getElementById("answerContent");
    const content = ta.value.trim();

    if (!content) return alert("Nhập nội dung trả lời");

    // Build object answer mới
    const newAnswer = {
        answer_id: "A" + ((topic.answers || []).length + 1),
        content,
        created_by: googleUser.name || "Bạn",
        created_at: new Date().toLocaleString(),
        likes: 0,
        picture: googleUser.picture || "https://via.placeholder.com/40",
    };

    // Thêm vào đầu mảng và lưu lại
    topic.answers = topic.answers || [];
    topic.answers.unshift(newAnswer);
    localStorage.setItem("classes", JSON.stringify(classes));
    // Hiển thị thông báo thành công
    showToast("✅ Câu trả lời đã được gửi thành công!", 4000, "success");
    // Reset textarea và re-render
    ta.value = "";
    ta.style.height = "auto";
    renderAnswers();
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

// Hàm showToast để hiển thị thông báo
function showToast(message, time = 3500, type = "warning") {
    const toast = document.getElementById("toast");
    toast.innerText = message;

    // Thay đổi màu sắc của toast tùy theo loại (warning, success, error)
    if (type === "success") {
        toast.style.backgroundColor = "#10b981"; // Màu xanh cho thành công
    } else if (type === "error") {
        toast.style.backgroundColor = "#ef4444"; // Màu đỏ cho lỗi
    } else {
        toast.style.backgroundColor = "#f59e0b"; // Màu vàng cho cảnh báo
    }

    toast.classList.remove("hidden");
    toast.classList.add("opacity-100");

    setTimeout(() => {
        toast.classList.add("hidden");
        toast.classList.remove("opacity-100");
    }, time);
}

document.getElementById(
    "discussionLink"
).href = `topic-detail.html?class_id=${encodeURIComponent(
    classId
)}&topic_id=${encodeURIComponent(topicId)}`;
document.getElementById(
    "thisHomeworkLink"
).href = `homework-list.html?class_id=${encodeURIComponent(
    classId
)}&topic_id=${encodeURIComponent(topicId)}`;
document.getElementById("infoStudents").innerText = (
    cls.memberList || []
).length;
document.getElementById("infoAssignments").innerText = (
    topic.homeworks || []
).length;
document.getElementById("infoDiscussions").innerText = (
    cls.topics || []
).length;