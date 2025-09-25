// Thêm vào đầu file để init biến
document.addEventListener('DOMContentLoaded', () => {
    window.cls = window.classData;
    window.students = window.studentsData;
    loadTopics();  // Gọi init
    renderMemberList();
    renderGroups();
});
let countdownTimers = [];
// Đặt min cho input datetime-local
const now = new Date();
const tzoffset = now.getTimezoneOffset() * 60000; // bù múi giờ
const localISOTime = new Date(now - tzoffset).toISOString().slice(0, 16);
document.getElementById("topicEndTime").setAttribute("min", localISOTime);

// Load topics từ API khi init
async function loadTopics() {
    try {
        const response = await fetch(`/api/topics/byclass/${cls.ClassId}`);  // Sửa endpoint
        if (response.ok) {
            const topics = await response.json();
            cls.topics = topics.map(t => ({
                topic_id: t.TopicId.toString(),
                title: t.Title,
                description: t.Description,
                end_time: t.EndTime,
                created_by: t.CreatedBy,
                created_at: t.CreatedAt,
                answers: []
            }));
            renderTopics();
        }
    } catch (error) {
        console.error('Lỗi load topics:', error);
    }
}

function renderTopics() {
    // Xóa hết timer cũ trước khi render lại
    countdownTimers.forEach((id) => clearTimeout(id));
    countdownTimers = [];
    const topicsList = document.getElementById("topicsList");
    if (!cls.topics || cls.topics.length === 0) {
        topicsList.innerHTML = "<div class='text-gray-500'>There are no topics yet.</div>";
        return;
    }
    topicsList.innerHTML = cls.topics.map((t, idx) => `
                <div class="p-4 border border-gray-300 rounded-lg shadow-sm hover:shadow-md transition duration-200 bg-white relative" onclick="goToTopic('${cls.classId}','${t.topic_id}')">
                    <button class="topic-delete-btn text-red-500 hover:text-red-700 font-bold text-lg" title="Delete topic" onclick="event.stopPropagation(); deleteTopic(${idx}); return false;">×</button>
                    <div class="text-gray-900 font-semibold text-base">${t.title}</div>
                    <div class="text-gray-500 text-sm">by ${t.created_by}</div>
                    <div class="text-gray-500 text-sm">
                        <b>End time:</b> <span id="topic-end-${idx}">${t.end_time ? new Date(t.end_time).toLocaleString() : "Not set"}</span>
                        <span class="ml-2 text-red-600" id="countdown-${idx}"></span>
                    </div>
                    <div class="text-gray-500 text-sm flex justify-end space-x-4">
                        <span>${t.answers ? t.answers.length : 0} replies</span>
                        <span>${t.created_at ? new Date(t.created_at).toLocaleString() : ""}</span>
                    </div>
                </div>
            `).join("");
    // Countdown
    cls.topics.forEach((t, idx) => {
        if (t.end_time && t.created_at) {
            startCountdown(t.end_time, `countdown-${idx}`, t.created_at, idx);
        }
    });
}

window.deleteTopic = function (idx) {
    if (!confirm("Are you sure you want to delete this topic?")) return;
    cls.topics.splice(idx, 1);
    renderTopics();
};

// Cập nhật thông tin số lượng thành viên và học viên
function updateMemberSection() {
    const memberCountEl = document.getElementById("memberCount");
    const studentCountEl = document.getElementById("studentCount");
    if (!cls.memberList || cls.memberList.length === 0) {
        memberCountEl.textContent = 0;
        studentCountEl.textContent = 0;
        return;
    }
    const totalMembers = cls.memberList.length;
    // Giả sử giáo viên là người có tên trùng với cls.teacher
    const studentCount = cls.memberList.filter(m => m.fullName !== cls.teacher).length;
    memberCountEl.textContent = totalMembers;
    studentCountEl.textContent = studentCount;
}

function renderMemberList() {
    const container = document.getElementById("memberListContainer");
    container.innerHTML = "";
    if (!cls.memberList) cls.memberList = [];
    cls.memberList.forEach((member, index) => {
        const isTeacher = member.fullName === cls.teacher;
        const div = document.createElement("div");
        div.className = "p-4 bg-blue-50 rounded-lg flex items-center justify-between";
        div.innerHTML = `
                    <div>
                        <div class="flex items-center space-x-2">
                            <span class="bg-gray-200 text-xs w-7 h-7 flex items-center justify-center rounded-full">
                                ${member.fullName.split(" ").map(w => w[0]).join("").toUpperCase()}
                            </span>
                            <span class="font-medium">${member.fullName}</span>
                            <span class="text-gray-500 text-sm">${isTeacher ? "Teacher" : "Student"}</span>
                        </div>
                        <div class="text-gray-500 text-sm">${isTeacher ? "Instructor" : ""}</div>
                    </div>
                    ${!isTeacher ? `<button class="text-red-600 text-sm hover:underline" onclick="removeMemberById('${member.userId}')">❌ Remove</button>` : ""}
                `;
        container.appendChild(div);
    });
    updateMemberSection(); // cập nhật số lượng hiển thị
}

const addMemberModal = document.getElementById("addMemberModal");
const availableStudentsList = document.getElementById("availableStudentsList");
const cancelAddMember = document.getElementById("cancelAddMember");
const confirmAddMember = document.getElementById("confirmAddMember");
// Hiển thị modal khi bấm nút "Add Member"
document.getElementById("addMemberBtn").addEventListener("click", () => {
    const currentIds = cls.memberList.map(m => m.userId);
    const availableToAdd = students.filter(s => !currentIds.includes(s.userId));
    if (availableToAdd.length === 0) {
        alert("Không còn học viên nào để thêm.");
        return;
    }
    availableStudentsList.innerHTML = availableToAdd.map((s, i) => `
                <label class="flex items-center space-x-2">
                    <input type="checkbox" value="${s.userId}" class="studentCheckbox" />
                    <span>${s.fullName}</span>
                </label>
            `).join("");
    addMemberModal.classList.remove("hidden");
});
// Hủy thêm
cancelAddMember.addEventListener("click", () => {
    addMemberModal.classList.add("hidden");
});
// Xác nhận thêm thành viên
confirmAddMember.addEventListener("click", () => {
    const checkedBoxes = document.querySelectorAll(".studentCheckbox:checked");
    const selectedIds = Array.from(checkedBoxes).map(cb => cb.value);
    if (selectedIds.length === 0) {
        alert("No student selected.");
        return;
    }
    const toAdd = students.filter(s => selectedIds.includes(s.userId)).map(s => ({ userId: s.userId, fullName: s.fullName }));
    cls.memberList.push(...toAdd);
    renderMemberList();
    addMemberModal.classList.add("hidden");
});
// Tab switching
document.getElementById("discussionsTab").addEventListener("click", () => {
    document.getElementById("discussionsSection").classList.remove("hidden");
    document.getElementById("membersSection").classList.add("hidden");
    document.getElementById("resourcesSection").classList.add("hidden");
    document.getElementById("groupsSection").classList.add("hidden");
    // Active tab
    document.getElementById("discussionsTab").classList.add("bg-white", "border", "text-gray-900");
    document.getElementById("discussionsTab").classList.remove("bg-gray-200", "text-gray-600");
    // Inactive tabs
    document.getElementById("membersTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("membersTab").classList.add("bg-gray-200", "text-gray-600");
    document.getElementById("resourcesTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("resourcesTab").classList.add("bg-gray-200", "text-gray-600");
    document.getElementById("groupsTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("groupsTab").classList.add("bg-gray-200", "text-gray-600");
});
document.getElementById("membersTab").addEventListener("click", () => {
    document.getElementById("discussionsSection").classList.add("hidden");
    document.getElementById("membersSection").classList.remove("hidden");
    document.getElementById("resourcesSection").classList.add("hidden");
    document.getElementById("groupsSection").classList.add("hidden");
    document.getElementById("membersTab").classList.add("bg-white", "border", "text-gray-900");
    document.getElementById("membersTab").classList.remove("bg-gray-200", "text-gray-600");
    document.getElementById("discussionsTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("discussionsTab").classList.add("bg-gray-200", "text-gray-600");
    document.getElementById("resourcesTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("resourcesTab").classList.add("bg-gray-200", "text-gray-600");
    document.getElementById("groupsTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("groupsTab").classList.add("bg-gray-200", "text-gray-600");
});
document.getElementById("resourcesTab").addEventListener("click", () => {
    document.getElementById("discussionsSection").classList.add("hidden");
    document.getElementById("membersSection").classList.add("hidden");
    document.getElementById("resourcesSection").classList.remove("hidden");
    document.getElementById("groupsSection").classList.add("hidden");
    document.getElementById("resourcesTab").classList.add("bg-white", "border", "text-gray-900");
    document.getElementById("resourcesTab").classList.remove("bg-gray-200", "text-gray-600");
    document.getElementById("discussionsTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("discussionsTab").classList.add("bg-gray-200", "text-gray-600");
    document.getElementById("membersTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("membersTab").classList.add("bg-gray-200", "text-gray-600");
    document.getElementById("groupsTab").classList.remove("bg-white", "border", "text-gray-900");
    document.getElementById("groupsTab").classList.add("bg-gray-200", "text-gray-600");
});
document.getElementById("groupsTab").addEventListener("click", () => {
    document.getElementById("discussionsSection").classList.add("hidden");
    document.getElementById("membersSection").classList.add("hidden");
    document.getElementById("resourcesSection").classList.add("hidden");
    document.getElementById("groupsSection").classList.remove("hidden");
    // Active tab
    document.getElementById("groupsTab").classList.add("bg-white", "border", "text-gray-900");
    document.getElementById("groupsTab").classList.remove("bg-gray-200", "text-gray-600");
    // Inactive tabs
    ["discussionsTab", "membersTab", "resourcesTab"].forEach((id) => {
        document.getElementById(id).classList.remove("bg-white", "border", "text-gray-900");
        document.getElementById(id).classList.add("bg-gray-200", "text-gray-600");
    });
});
// Đặt mặc định là Discussions sau khi gán xong sự kiện
document.getElementById("discussionsTab").click();

window.goToTopic = function (classId, topicId) {
    window.location.href = `topic-detail?class_id=${classId}&topic_id=${topicId}`;
};

// Thêm chủ đề mới - Gọi API
document.getElementById("addTopicBtn").onclick = async () => {
    const title = document.getElementById("topicTitle").value.trim();
    const desc = document.getElementById("topicDesc").value.trim();
    const endTimeInput = document.getElementById("topicEndTime").value;

    if (!title) { alert("Enter a title."); return; }
    if (!endTimeInput) {
        alert("Please select an end time for the topic!");
        document.getElementById("topicEndTime").focus();
        return;
    }

    const endTimeDate = new Date(endTimeInput);
    if (endTimeDate <= new Date()) {
        alert("The end time must be later than the current time!");
        document.getElementById("topicEndTime").focus();
        return;
    }

    const endTimeISO = endTimeDate.toISOString();

    try {
        const response = await fetch('/api/topics', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                classId: parseInt(cls.ClassId),
                title: title,
                description: desc,
                type: "discussion",
                endTime: endTimeISO,
                createdBy: cls.CreatedByNavigation?.FullName || "Nguyen Van A"
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        await response.json();  // Confirm

        loadTopics();  // Reload từ DB

        // Reset form
        document.getElementById("topicTitle").value = "";
        document.getElementById("topicDesc").value = "";
        document.getElementById("topicEndTime").value = "";

        alert("Topic created successfully!");
    } catch (error) {
        console.error('Error creating topic:', error);
        alert("Failed to create topic. Please try again.");
    }
};


// Xóa thành viên khỏi lớp
window.removeMemberById = function (id) {
    if (!confirm("Are you sure you want to remove this member from the class?")) return;
    cls.memberList = cls.memberList.filter(member => member.userId !== id);
    renderMemberList();
};

// ==== GROUP MANAGEMENT ====
function renderGroups() {
    if (!cls.groups) cls.groups = [];
    const groupList = document.getElementById("groupList");
    const groupCountEl = document.getElementById("groupCount");
    groupCountEl.textContent = cls.groups.length;
    groupList.innerHTML = "";
    cls.groups.forEach((group, index) => {
        const groupDiv = document.createElement("div");
        groupDiv.className = "border border-gray-300 p-4 rounded cursor-pointer hover:shadow transition";
        const membersHtml = group.members.map(id => {
            const m = cls.memberList.find(mem => mem.userId === id);
            return m ? `<li>${m.fullName}</li>` : "";
        }).join("");
        const availableToAdd = cls.memberList.filter(m => !group.members.includes(m.userId));
        groupDiv.innerHTML = `
                    <div class="flex justify-between items-center mb-2">
                        <h4 class="font-semibold">Group ${index + 1}</h4>
                        <!-- CHẶN NỔI BỌT Ở NÚT XÓA -->
                        <button onclick="event.stopPropagation(); deleteGroup(${index})" class="text-red-500 hover:text-red-700 font-bold text-lg" title="Xóa nhóm">×</button>
                    </div>
                    <ul class="ml-4 text-sm text-gray-700">
                        ${membersHtml || "<li><em>No members</em></li>"}
                    </ul>
                    ${availableToAdd.length > 0 ? `
                        <!-- CHẶN NỔI BỌT Ở SELECT -->
                        <select id="addMemberSelect_${index}" class="mt-2 border p-1 rounded text-sm" onclick="event.stopPropagation()" onmousedown="event.stopPropagation()" onchange="event.stopPropagation()">
                            <option value="">+ Add Member</option>
                            ${availableToAdd.map(m => `<option value="${m.userId}">${m.fullName}</option>`).join("")}
                        </select>
                    ` : ""}
                `;
        // Gán change handler cho select (NHỚ stopPropagation trong handler)
        if (availableToAdd.length > 0) {
            const sel = groupDiv.querySelector(`#addMemberSelect_${index}`);
            sel.addEventListener("change", (e) => {
                e.stopPropagation(); // quan trọng!
                const selectedId = e.target.value;
                if (!selectedId) return;
                cls.groups[index].members.push(selectedId);
                renderGroups();
            });
        }
        // Chỉ điều hướng khi click vào khoảng trống của thẻ group (không phải button/select)
        groupDiv.addEventListener("click", (e) => {
            // Nếu click vào phần tử tương tác thì bỏ qua
            if (e.target.closest("button, select, option, input, label, a")) return;
            window.location.href = `group-detail?class_id=${cls.classId}&group_index=${index}`;
        });
        groupList.appendChild(groupDiv);
    });
}
document.getElementById("createGroupBtn").addEventListener("click", () => {
    if (!cls.groups) cls.groups = [];
    cls.groups.push({ members: [] });
    renderGroups();
});
window.deleteGroup = function (index) {
    if (!confirm("Are you sure you want to delete this group?")) return;
    cls.groups.splice(index, 1);
    renderGroups();
};

function startCountdown(endTimeStr, countdownElemId, createdAtStr, idx) {
    const endTime = Date.parse(endTimeStr);
    let startTime;
    if (createdAtStr) {
        let t = Date.parse(createdAtStr);
        if (isNaN(t)) t = new Date(createdAtStr).getTime();
        startTime = t;
    } else {
        startTime = Date.now();
    }
    const total = endTime - startTime;
    function updateCountdown() {
        const now = Date.now();
        const diff = endTime - now;
        const el = document.getElementById(countdownElemId);
        if (!el) return;
        el.classList.remove("countdown-green", "countdown-yellow", "countdown-red", "countdown-expired");
        if (diff <= 0) {
            el.textContent = "Time is up!";
            el.classList.add("countdown-expired");
            return;
        }
        const d = Math.floor(diff / (1000 * 60 * 60 * 24));
        const h = Math.floor((diff / (1000 * 60 * 60)) % 24);
        const m = Math.floor((diff / (1000 * 60)) % 60);
        const s = Math.floor((diff / 1000) % 60);
        el.textContent = `Remaining: ${d > 0 ? d + " days " : ""}${h}h ${m}m ${s}s`;
        // Lưu id timer vào mảng countdownTimers theo index
        countdownTimers[idx] = setTimeout(updateCountdown, 1000);
    }
    updateCountdown();
}

// ======= RANKING FEATURE =======
// Hiển thị popup bảng xếp hạng khi bấm nút
document.getElementById("rankButton").addEventListener("click", () => {
    renderRankModal();
    document.getElementById("rankModal").classList.remove("hidden");
});
// Ẩn popup khi bấm nút đóng
document.getElementById("closeRankModal").addEventListener("click", () => {
    document.getElementById("rankModal").classList.add("hidden");
});
// Hàm render nội dung bảng xếp hạng vào modal
function renderRankModal(page = 1) {
    const container = document.getElementById("rankModalContent");
    container.innerHTML = ""; // Xóa nội dung cũ
    const rankedMembers = [...cls.memberList].sort((a, b) => (b.rating || 0) - (a.rating || 0));
    if (rankedMembers.length === 0) {
        container.innerHTML = `<div class="text-gray-500 text-center py-4">Chưa có thành viên nào.</div>`;
        return;
    }
    // Cài đặt phân trang
    const itemsPerPage = 8;
    const totalPages = Math.ceil(rankedMembers.length / itemsPerPage);
    const startIndex = (page - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentMembers = rankedMembers.slice(startIndex, endIndex);
    // SVG crown icon cho top 1
    const crown = `<svg class="rank-crown" width="24" height="20" viewBox="0 0 24 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M2 18H22L20.5 11L15 14L12 8L9 14L3.5 11L2 18Z" fill="#FFD700" stroke="#DAA520" stroke-width="1.5"/>
            </svg>`;
    // Tạo HTML cho bảng xếp hạng
    const tableHTML = `
                <div class="overflow-x-auto">
                    <table class="rank-table w-full text-left">
                        <thead>
                            <tr class="bg-gray-100">
                                <th class="py-3 px-4 w-12 text-center font-semibold text-gray-700 border-b border-gray-200">Rank</th>
                                <th class="py-3 px-4 font-semibold text-gray-700 border-b border-gray-200">Member</th>
                                <th class="py-3 px-4 text-center font-semibold text-gray-700 border-b border-gray-200">Stars</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${currentMembers.map((m, i) => {
        const globalIndex = startIndex + i + 1;
        return `
                                    <tr class="${globalIndex === 1 ? "top-1" : globalIndex === 2 ? "top-2" : globalIndex === 3 ? "top-3" : ""}">
                                        <td class="py-3 px-4 text-center font-medium text-gray-800 border-b border-gray-200">${globalIndex}</td>
                                        <td class="py-3 px-4 flex items-center gap-3 border-b border-gray-200">
                                            ${globalIndex === 1 ? crown : ""}
                                            <span class="font-medium ${globalIndex <= 3 ? "text-gray-900" : "text-gray-700"}">${m.fullName}</span>
                                        </td>
                                        <td class="py-3 px-4 text-center font-medium border-b border-gray-200">
                                            ${m.rating || 0}
                                            <span class="rank-star">★</span>
                                        </td>
                                    </tr>
                                `;
    }).join("")}
                        </tbody>
                    </table>
                </div>
            `;
    // Create HTML for pagination
    const paginationHTML = `
                <div class="flex justify-between items-center mt-4">
                    <div class="text-sm text-gray-500">
                        Showing ${startIndex + 1}-${Math.min(endIndex, rankedMembers.length)} of ${rankedMembers.length} members
                    </div>
                    <div class="flex gap-2">
                        <button class="pagination-btn px-3 py-1.5 rounded-md text-sm ${page === 1 ? "bg-gray-200 text-gray-500 cursor-not-allowed" : "bg-blue-600 text-white hover:bg-blue-700"}"
                                ${page === 1 ? "disabled" : ""}
                                onclick="renderRankModal(${page - 1})">Previous</button>
                        <div class="flex gap-1">
                            ${Array.from({ length: totalPages }, (_, i) => `
                                <button class="pagination-btn px-3 py-1.5 rounded-md text-sm ${page === i + 1 ? "bg-blue-600 text-white" : "bg-gray-100 text-gray-700 hover:bg-gray-200"}"
                                        onclick="renderRankModal(${i + 1})">${i + 1}</button>
                            `).join("")}
                        </div>
                        <button class="pagination-btn px-3 py-1.5 rounded-md text-sm ${page === totalPages ? "bg-gray-200 text-gray-500 cursor-not-allowed" : "bg-blue-600 text-white hover:bg-blue-700"}"
                                ${page === totalPages ? "disabled" : ""}
                                onclick="renderRankModal(${page + 1})">Next</button>
                    </div>
                </div>
            `;
    container.innerHTML = `
                ${tableHTML}
                ${paginationHTML}
                <div class="text-xs text-gray-400 text-right mt-2">* Top 1, 2, 3 are highlighted</div>
            `;
}

// ==== REPORT FEATURE ====
document.getElementById("reportButton").addEventListener("click", () => {
    renderReportModal();
    document.getElementById("reportModal").classList.remove("hidden");
});
document.getElementById("closeReportModal").addEventListener("click", () => {
    document.getElementById("reportModal").classList.add("hidden");
});
function renderReportModal() {
    const container = document.getElementById("reportModalContent");
    container.innerHTML = "";
    if (!reportData || reportData.length === 0) {
        container.innerHTML = `<div class="text-gray-500 text-center py-4">No report data available.</div>`;
        return;
    }
    // Đếm số lần Negative, Neutral, Positive cho mỗi học viên
    const summary = {};
    reportData.forEach((item) => {
        const name = item.senderName;
        if (!summary[name]) {
            summary[name] = { Negative: 0, Neutral: 0, Positive: 0 };
        }
        if (summary[name][item.engagement] !== undefined) {
            summary[name][item.engagement]++;
        }
    });
    // Tính điểm chung từ 3 giá trị
    // Ví dụ quy đổi:
    // Negative = 0 điểm
    // Neutral = 50 điểm
    // Positive = 100 điểm
    // Điểm trung bình = (Negative*0 + Neutral*50 + Positive*100) / (Tổng lượt)
    // Tính điểm participation points (0–100)
    function calcParticipationPoints(counts) {
        const total = counts.Negative + counts.Neutral + counts.Positive;
        if (total === 0) return 0;
        const score = (counts.Negative * 0 + counts.Neutral * 50 + counts.Positive * 100) / total;
        return Math.round(score);
    }
    // Tính điểm tổng kết
    function getLetterGrade(score) {
        if (score >= 97) return "A+";
        if (score >= 93) return "A";
        if (score >= 90) return "A−";
        if (score >= 87) return "B+";
        if (score >= 83) return "B";
        if (score >= 80) return "B−";
        if (score >= 77) return "C+";
        if (score >= 73) return "C";
        if (score >= 70) return "C−";
        if (score >= 67) return "D+";
        if (score >= 63) return "D";
        if (score >= 60) return "D−";
        return "F";
    }
    // Render bảng mới chỉ có 2 cột: Name + Participation Points + Nhận xét
    const tableHTML = `
                <div class="overflow-x-auto">
                    <table class="min-w-full border border-gray-200">
                        <thead class="bg-gray-100">
                            <tr>
                                <th class="py-2 px-4 border">Name</th>
                                <th class="py-2 px-4 border text-center">Participation Points</th>
                                <th class="py-2 px-4 border text-center">Letter Grade</th>
                                <th class="py-2 px-4 border text-center">Overall Assessment</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${Object.entries(summary).map(([name, counts]) => {
        const points = calcParticipationPoints(counts);
        const grade = getLetterGrade(points);
        let comment = "";
        if (points >= 80) comment = "Very good and active student";
        else if (points >= 60) comment = "There is an effort but you need to work more critically";
        else comment = "Needs much more engagement";
        return `
                                    <tr>
                                        <td class="py-2 px-4 border">${name}</td>
                                        <td class="py-2 px-4 border text-center">${points}%</td>
                                        <td class="py-2 px-4 border text-center">${grade}</td>
                                        <td class="py-2 px-4 border text-center">${comment}</td>
                                    </tr>
                                `;
    }).join("")}
                        </tbody>
                    </table>
                </div>
            `;
    container.innerHTML = tableHTML;
}

// Init
loadTopics();
renderMemberList();
renderGroups();