document.addEventListener("DOMContentLoaded", () => {
    console.log("Class:", window.CurrentClass);
    console.log("Topic:", window.CurrentTopic);
    console.log("Homeworks:", window.Homeworks);

    if (!window.CurrentClass || !window.CurrentTopic) {
        console.error("Thiếu dữ liệu CurrentClass hoặc CurrentTopic từ Razor.");
        return;
    }

    const classId = window.CurrentClass.classId;
    const topicId = window.CurrentTopic.topicId;

    //// Sidebar link
    //document.getElementById("discussionLink").href = `/Class/TopicDetail?class_id=${classId}&topic_id=${topicId}`;
    //document.getElementById("thisHomeworkLink").href = `/Class/HomeworkList?class_id=${classId}&topic_id=${topicId}`;

    // Render UI
    renderClassInfo();
    //renderHomeworkList();
    // các nút modal tạo bài tập
    const createBtn = document.getElementById("openCreateHomework");
    const modal = document.getElementById("createHomeworkModal");
    const closeBtn = document.getElementById("closeCreateHomework");
    const cancelBtn = document.getElementById("cancelCreateHomework");

    if (createBtn && modal) {
        createBtn.addEventListener("click", (e) => {
            e.preventDefault();
            modal.classList.remove("hidden"); // mở modal
        });
    }

    if (closeBtn && modal) {
        closeBtn.addEventListener("click", () => {
            modal.classList.add("hidden"); // đóng khi nhấn dấu ×
        });
    }

    if (cancelBtn && modal) {
        cancelBtn.addEventListener("click", (e) => {
            e.preventDefault();
            modal.classList.add("hidden"); // đóng khi nhấn Cancel
        });
    }

    // Thêm: click ra ngoài modal thì cũng tắt
    window.addEventListener("click", (e) => {
        if (e.target === modal) {
            modal.classList.add("hidden");
        }
    });

});

function renderClassInfo() {
    const infoDiv = document.getElementById("classInfo");
    if (!infoDiv) return;

    const cls = window.CurrentClass;
    const topic = window.CurrentTopic;

    infoDiv.innerHTML = `
        <h1 class="text-2xl font-bold text-blue-700 mb-2">${cls.className}</h1>
        <p class="text-gray-600">Subject Code: <b>${cls.subjectCode}</b></p>
        <p class="text-gray-600">Semester: <b>${cls.semester ?? "N/A"}</b></p>
        <p class="text-gray-600 mt-2">Topic: <b>${topic.title}</b></p>
        <p class="text-gray-500 text-sm">${topic.description ?? ""}</p>
    `;
}

//function renderHomeworkList() {
//    const listDiv = document.getElementById("homeworkList");
//    if (!listDiv) return;

//    const homeworks = window.Homeworks || [];

//    if (homeworks.length === 0) {
//        listDiv.innerHTML = `<div class="text-gray-500 italic">There are no homework assignments yet.</div>`;
//        return;
//    }

//    listDiv.innerHTML = homeworks.map(hw => `
//        <div class="p-4 bg-white border rounded-lg shadow-sm hover:shadow-md transition">
//            <div class="flex justify-between items-center mb-2">
//                <h3 class="text-lg font-semibold text-blue-700">${hw.title}</h3>
//                <span class="text-sm text-gray-500">Deadline: ${hw.dueDate ? new Date(hw.dueDate).toLocaleString() : "N/A"}</span>
//            </div>
//            <p class="text-gray-700 mb-2">${hw.description ?? "No description provided."}</p>
//            <div class="text-right">
//                <a href="/Class/HomeworkDetail?homework_id=${hw.homeworkId}"
//                   class="text-sm text-white bg-blue-600 hover:bg-blue-700 px-3 py-1.5 rounded">
//                   View Details
//                </a>
//            </div>
//        </div>
//    `).join("");
//}
