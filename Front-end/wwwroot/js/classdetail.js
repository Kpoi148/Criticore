// Định nghĩa tất cả các hàm và biến toàn cục trước để tránh lỗi hoisting/reference
let countdownTimers = [];
window.cls = window.cls || {}; // Đảm bảo cls tồn tại
window.students = window.studentsData || []; // Khởi tạo từ dữ liệu Razor
window.reportData = window.reportData || []; // Khởi tạo reportData nếu cần
// Thống nhất tên: Sử dụng cls.members thay vì trộn lẫn cls.memberList và cls.Members
if (window.cls.Members) {
    // Thống nhất property names sang lowercase (camelCase) để tránh lỗi undefined
    window.cls.members = window.cls.Members.map(m => ({
        userId: m.UserId,
        fullName: m.FullName,
        roleInClass: m.RoleInClass // Thêm nếu cần, lowercase
    }));
} else {
    window.cls.members = [];
}
// Định nghĩa goToTopic sớm
window.goToTopic = function (classId, topicId) {
    window.location.href = `TopicDetail?class_id=${classId}&topic_id=${topicId}`;
};
// Định nghĩa deleteTopic sớm
window.deleteTopic = async function (idx) {
    if (!await Swal.fire({
        icon: 'question',
        title: 'Xác nhận',
        text: 'Bạn có chắc muốn xóa topic này?',
        showCancelButton: true,
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => result.isConfirmed)) return;
    const topicId = cls.topics[idx].topic_id; // Lấy ID topic
    try {
        const response = await fetch(`https://localhost:7193/api/Topics/${topicId}`
        //const response = await fetch(`https://class.criticore.edu.vn:8005/api/Topics/${topicId}`
            , {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' }
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        cls.topics.splice(idx, 1); // Xóa cục bộ sau khi API thành công
        renderTopics();
        loadTopics(); // Reload từ DB để đồng bộ
        Swal.fire({ icon: 'success', title: 'Thành công', text: 'Topic đã xóa!', timer: 1500 });
    } catch (error) {
        console.error('Error deleting topic:', error);
        Swal.fire({ icon: 'error', title: 'Lỗi', text: 'Không thể xóa topic. Vui lòng thử lại.' });
    }
};
// Định nghĩa removeMemberById sớm
window.removeMemberById = function (id) {
    if (!confirm("Are you sure you want to remove this member from the class?")) return;
    cls.members = cls.members.filter(member => member.userId !== id);
    renderMemberList();
};
// Định nghĩa deleteGroup sớm
window.deleteGroup = function (index) {
    if (!confirm("Are you sure you want to delete this group?")) return;
    cls.groups.splice(index, 1);
    renderGroups();
};
// Các hàm khác
function initTopicEndTime() {
    const topicEndInput = document.getElementById("topicEndTime");
    if (topicEndInput) {
        const now = new Date();
        const tzoffset = now.getTimezoneOffset() * 60000;
        const localISOTime = new Date(now - tzoffset).toISOString().slice(0, 16);
        topicEndInput.setAttribute("min", localISOTime);
    } else {
        console.warn("⚠️ Không tìm thấy #topicEndTime trong DOM (có thể form chưa render).");
    }
}
async function loadTopics() {
    try {
        const response = await fetch(`https://localhost:7193/api/Topics/byclass/${cls.ClassId}`);
        //const response = await fetch(`https://class.criticore.edu.vn:8005/api/Topics/byclass/${cls.ClassId}`);
        if (response.ok) {
            let topics = await response.json();
            cls.topics = await Promise.all(topics.map(async (t) => {
                try {
                    const ansResponse = await fetch(`https://localhost:7134/api/TopicDetail/topics/${t.topicId}/answers`);
                    //const ansResponse = await fetch(`https://topicdetail.criticore.edu.vn:8009/api/TopicDetail/topics/${t.topicId}/answers`);
                    const answers = ansResponse.ok ? await ansResponse.json() : [];
                    return {
                        topic_id: t.topicId ? t.topicId.toString() : '',
                        title: t.title,
                        description: t.description,
                        end_time: t.endTime,
                        created_by: t.createdBy, // Giữ ID nếu cần cho logic khác
                        created_by_name: t.createdByName || 'Unknown', // Thêm tên người tạo từ DTO
                        created_at: t.createdAt,
                        answers: answers // Đã có rating và userId trong answers
                    };
                } catch (err) {
                    console.error(`Lỗi fetch answers cho topic ${t.topicId}:`, err);
                    return { ...t, answers: [] };
                }
            }));
            renderTopics(); // Giả định renderTopics() sẽ sử dụng created_by_name để hiển thị tên
            // Thêm: Tính rating sau khi load đầy đủ
            await calculateMemberRatings();
            // Optional: Re-render ranking nếu modal mở
            if (!document.getElementById("rankModal").classList.contains("hidden")) {
                renderRankModal();
            }
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
    if (!topicsList) return; // Tránh lỗi nếu element chưa tồn tại
    if (!cls.topics || cls.topics.length === 0) {
        topicsList.innerHTML = "<div class='text-gray-500'>There are no topics yet.</div>";
        return;
    }
    topicsList.innerHTML = cls.topics.map((t, idx) => `
                <div class="p-4 border border-gray-300 rounded-lg shadow-sm hover:shadow-md transition duration-200 bg-white relative" onclick="goToTopic('${cls.ClassId}','${t.topic_id}')">
                    ${window.isTeacher ? `<button class="topic-delete-btn text-red-500 hover:text-red-700 font-bold text-lg" title="Delete topic" onclick="event.stopPropagation(); deleteTopic(${idx}); return false;">×</button>` : ''}
                    <div class="text-gray-900 font-semibold text-base">${t.title}</div>
                    <div class="text-gray-500 text-sm">by ${t.created_by_name || 'Unknown'}</div>
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
function updateMemberSection() {
    const memberCountEl = document.getElementById("memberCount");
    const studentCountEl = document.getElementById("studentCount");
    if (!cls.members || cls.members.length === 0) {
        if (memberCountEl) memberCountEl.textContent = 0;
        if (studentCountEl) studentCountEl.textContent = 0;
        return;
    }
    const totalMembers = cls.members.length;
    // Giả sử giáo viên là người có tên trùng với cls.teacher
    const studentCount = cls.members.filter(m => m.fullName !== cls.teacher).length;
    if (memberCountEl) memberCountEl.textContent = totalMembers;
    if (studentCountEl) studentCountEl.textContent = studentCount;
}
function renderMemberList() {
    const container = document.getElementById("memberListContainer");
    if (!container) return; // Tránh lỗi nếu chưa tồn tại
    container.innerHTML = "";
    if (!cls.members) cls.members = []; // Fallback nếu undefined
    cls.members.forEach((member, index) => {
        const isTeacher = member.roleInClass === "Teacher"; // Dùng roleInClass lowercase
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
            ${!isTeacher && window.isTeacher ? `<button class="text-red-600 text-sm hover:underline" onclick="removeMemberById('${member.userId}')">❌ Remove</button>` : ""}
        `;
        container.appendChild(div);
    });
    updateMemberSection(); // cập nhật số lượng hiển thị
}
function toggleAddTopicForm() {
    const form = document.getElementById("addTopicForm");
    if (!form) return;
    form.style.display = form.style.display === "none" ? "block" : "none";
    if (form.style.display === "block") {
        initTopicEndTime(); // 👉 Set min khi form hiển thị
    }
}
function renderGroups() {
    if (!cls.groups) cls.groups = [];
    const groupList = document.getElementById("groupList");
    if (!groupList) return;
    const groupCountEl = document.getElementById("groupCount");
    if (groupCountEl) groupCountEl.textContent = cls.groups.length;
    groupList.innerHTML = "";
    cls.groups.forEach((group, index) => {
        const groupDiv = document.createElement("div");
        groupDiv.className = "border border-gray-300 p-4 rounded cursor-pointer hover:shadow transition";
        const membersHtml = group.members.map(id => {
            const m = cls.members.find(mem => mem.userId === id);
            return m ? `<li>${m.fullName}</li>` : "";
        }).join("");
        const availableToAdd = cls.members.filter(m => !group.members.includes(m.userId));
        groupDiv.innerHTML = `
                    <div class="flex justify-between items-center mb-2">
                        <h4 class="font-semibold">Group ${index + 1}</h4>
                        <!-- CHẶN NỔI BỌT Ở NÚT XÓA -->
                        ${window.isTeacher ? `<button onclick="event.stopPropagation(); deleteGroup(${index})" class="text-red-500 hover:text-red-700 font-bold text-lg" title="Xóa nhóm">×</button>` : ''}
                    </div>
                    <ul class="ml-4 text-sm text-gray-700">
                        ${membersHtml || "<li><em>No members</em></li>"}
                    </ul>
                    ${availableToAdd.length > 0 && window.isTeacher ? `
                        <!-- CHẶN NỔI BỌT Ở SELECT -->
                        <select id="addMemberSelect_${index}" class="mt-2 border p-1 rounded text-sm" onclick="event.stopPropagation()" onmousedown="event.stopPropagation()" onchange="event.stopPropagation()">
                            <option value="">+ Add Member</option>
                            ${availableToAdd.map(m => `<option value="${m.userId}">${m.fullName}</option>`).join("")}
                        </select>
                    ` : ""}
                `;
        // Gán change handler cho select (NHỚ stopPropagation trong handler)
        if (availableToAdd.length > 0 && window.isTeacher) {
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
function renderRankModal(page = 1) {
    const container = document.getElementById("rankModalContent");
    if (!container) return;
    container.innerHTML = ""; // Xóa nội dung cũ
    const rankedMembers = [...cls.members].sort((a, b) => (b.rating || 0) - (a.rating || 0));
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
                                            ${m.rating.toFixed(1) || 0} <span class="rank-star">★</span> <!-- Format 1 chữ số thập phân -->
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
function renderReportModal() {
    const container = document.getElementById("reportModalContent");
    if (!container) return;
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
async function calculateMemberRatings() {
    if (!cls.members || cls.members.length === 0 || !cls.topics) return;
    // Reset rating cũ
    cls.members.forEach(m => m.rating = 0);
    // Object để track tổng rating và count per userId
    const userRatings = {};
    cls.members.forEach(m => {
        userRatings[m.userId] = { totalRating: 0, count: 0 };
    });
    // Duyệt qua tất cả topics và answers
    cls.topics.forEach(topic => {
        if (topic.answers && topic.answers.length > 0) {
            topic.answers.forEach(answer => {
                const userId = answer.userId; // Từ AnswerDto
                if (userRatings[userId]) {
                    const rating = answer.rating || 0; // Từ API, average sao
                    userRatings[userId].totalRating += rating;
                    userRatings[userId].count++;
                }
            });
        }
    });
    // Gán average rating vào members
    cls.members.forEach(m => {
        const stats = userRatings[m.userId];
        if (stats.count > 0) {
            m.rating = Math.round((stats.totalRating / stats.count) * 10) / 10; // Round 1 chữ số thập phân
        } else {
            m.rating = 0;
        }
    });
    // Optional: Sort members theo rating descending để dễ dùng ở ranking
    cls.members.sort((a, b) => (b.rating || 0) - (a.rating || 0));
}
// DOMContentLoaded: Chỉ gọi init ở đây để đảm bảo DOM ready
document.addEventListener('DOMContentLoaded', () => {
    window.students = window.studentsData;
    if (!window.cls) {
        console.error('window.cls is undefined. Check Model serialization in Razor Page.');
        return; // Dừng nếu undefined
    }
    console.log('ClassId:', cls.ClassId);
    loadTopics(); // Gọi init
    renderMemberList();
    renderGroups();
    // Thêm: Ẩn phần tạo topic nếu không phải teacher (dù đã conditional render, nhưng để chắc chắn)
    if (!window.isTeacher) {
        const addTopicForm = document.getElementById("addTopicForm");
        if (addTopicForm) addTopicForm.style.display = "none";
    }
    // Thêm event listener cho nút toggle form tạo topic (nếu tồn tại)
    const toggleAddTopic = document.getElementById("toggleAddTopic");
    if (toggleAddTopic) {
        toggleAddTopic.addEventListener("click", toggleAddTopicForm);
    }
    // Add Member Modal logic
    const addMemberModal = document.getElementById("addMemberModal");
    const availableStudentsList = document.getElementById("availableStudentsList");
    const cancelAddMember = document.getElementById("cancelAddMember");
    const confirmAddMember = document.getElementById("confirmAddMember");
    // Hiển thị modal khi bấm nút "Add Member"
    const addMemberBtn = document.getElementById("addMemberBtn");
    if (addMemberBtn) {
        addMemberBtn.addEventListener("click", () => {
            const currentIds = cls.members.map(m => m.userId);
            const availableToAdd = students.filter(s => !currentIds.includes(s.userId));
            if (availableToAdd.length === 0) {
                Swal.fire({ icon: 'info', title: 'Thông báo', text: 'Không còn học viên nào để thêm.' });
                return;
            }
            if (availableStudentsList) {
                availableStudentsList.innerHTML = availableToAdd.map((s, i) => `
                    <label class="flex items-center space-x-2">
                        <input type="checkbox" value="${s.userId}" class="studentCheckbox" />
                        <span>${s.fullName}</span>
                    </label>
                `).join("");
            }
            if (addMemberModal) addMemberModal.classList.remove("hidden");
        });
    }
    // Hủy thêm
    if (cancelAddMember) {
        cancelAddMember.addEventListener("click", () => {
            if (addMemberModal) addMemberModal.classList.add("hidden");
        });
    }
    // Xác nhận thêm thành viên
    if (confirmAddMember) {
        confirmAddMember.addEventListener("click", () => {
            const checkedBoxes = document.querySelectorAll(".studentCheckbox:checked");
            const selectedIds = Array.from(checkedBoxes).map(cb => cb.value);
            if (selectedIds.length === 0) {
                Swal.fire({ icon: 'warning', title: 'Cảnh báo', text: 'Chưa chọn học viên nào.' });
                return;
            }
            const toAdd = students.filter(s => selectedIds.includes(s.userId)).map(s => ({ userId: s.userId, fullName: s.fullName, roleInClass: s.RoleInClass || 'Student' }));
            cls.members.push(...toAdd);
            renderMemberList();
            if (addMemberModal) addMemberModal.classList.add("hidden");
        });
    }
    // Tab switching
    const discussionsTab = document.getElementById("discussionsTab");
    if (discussionsTab) {
        discussionsTab.addEventListener("click", () => {
            document.getElementById("discussionsSection").classList.remove("hidden");
            document.getElementById("membersSection").classList.add("hidden");
            document.getElementById("resourcesSection").classList.add("hidden");
            document.getElementById("groupsSection").classList.add("hidden");
            // Active tab
            discussionsTab.classList.add("bg-white", "border", "text-gray-900");
            discussionsTab.classList.remove("bg-gray-200", "text-gray-600");
            // Inactive tabs
            document.getElementById("membersTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("membersTab").classList.add("bg-gray-200", "text-gray-600");
            document.getElementById("resourcesTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("resourcesTab").classList.add("bg-gray-200", "text-gray-600");
            document.getElementById("groupsTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("groupsTab").classList.add("bg-gray-200", "text-gray-600");
        });
    }
    const membersTab = document.getElementById("membersTab");
    if (membersTab) {
        membersTab.addEventListener("click", () => {
            document.getElementById("discussionsSection").classList.add("hidden");
            document.getElementById("membersSection").classList.remove("hidden");
            document.getElementById("resourcesSection").classList.add("hidden");
            document.getElementById("groupsSection").classList.add("hidden");
            membersTab.classList.add("bg-white", "border", "text-gray-900");
            membersTab.classList.remove("bg-gray-200", "text-gray-600");
            document.getElementById("discussionsTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("discussionsTab").classList.add("bg-gray-200", "text-gray-600");
            document.getElementById("resourcesTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("resourcesTab").classList.add("bg-gray-200", "text-gray-600");
            document.getElementById("groupsTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("groupsTab").classList.add("bg-gray-200", "text-gray-600");
        });
    }
    const resourcesTab = document.getElementById("resourcesTab");
    if (resourcesTab) {
        resourcesTab.addEventListener("click", () => {
            document.getElementById("discussionsSection").classList.add("hidden");
            document.getElementById("membersSection").classList.add("hidden");
            document.getElementById("resourcesSection").classList.remove("hidden");
            document.getElementById("groupsSection").classList.add("hidden");
            resourcesTab.classList.add("bg-white", "border", "text-gray-900");
            resourcesTab.classList.remove("bg-gray-200", "text-gray-600");
            document.getElementById("discussionsTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("discussionsTab").classList.add("bg-gray-200", "text-gray-600");
            document.getElementById("membersTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("membersTab").classList.add("bg-gray-200", "text-gray-600");
            document.getElementById("groupsTab").classList.remove("bg-white", "border", "text-gray-900");
            document.getElementById("groupsTab").classList.add("bg-gray-200", "text-gray-600");
        });
    }
    const groupsTab = document.getElementById("groupsTab");
    if (groupsTab) {
        groupsTab.addEventListener("click", () => {
            document.getElementById("discussionsSection").classList.add("hidden");
            document.getElementById("membersSection").classList.add("hidden");
            document.getElementById("resourcesSection").classList.add("hidden");
            document.getElementById("groupsSection").classList.remove("hidden");
            // Active tab
            groupsTab.classList.add("bg-white", "border", "text-gray-900");
            groupsTab.classList.remove("bg-gray-200", "text-gray-600");
            // Inactive tabs
            ["discussionsTab", "membersTab", "resourcesTab"].forEach((id) => {
                const el = document.getElementById(id);
                if (el) {
                    el.classList.remove("bg-white", "border", "text-gray-900");
                    el.classList.add("bg-gray-200", "text-gray-600");
                }
            });
        });
    }
    // Đặt mặc định là Discussions sau khi gán xong sự kiện
    if (discussionsTab) discussionsTab.click();
    // Thêm chủ đề mới - Gọi API (chỉ cho phép nếu là teacher)
    const addTopicBtn = document.getElementById("addTopicBtn");
    if (addTopicBtn) {
        addTopicBtn.onclick = async () => {
            if (!window.isTeacher) {
                Swal.fire({ icon: 'error', title: 'Lỗi', text: 'Chỉ giáo viên mới có quyền tạo topic.' });
                return;
            }
            const title = document.getElementById("topicTitle").value.trim();
            const desc = document.getElementById("topicDesc").value.trim();
            const endTimeInput = document.getElementById("topicEndTime").value;
            if (!title) {
                Swal.fire({ icon: 'warning', title: 'Cảnh báo', text: 'Vui lòng nhập tiêu đề.' });
                return;
            }
            if (!endTimeInput) {
                Swal.fire({ icon: 'warning', title: 'Cảnh báo', text: 'Vui lòng chọn thời gian kết thúc!' });
                document.getElementById("topicEndTime").focus();
                return;
            }
            const endTimeDate = new Date(endTimeInput);
            endTimeDate.setHours(endTimeDate.getHours() + 7); 
            if (endTimeDate <= new Date()) {
                Swal.fire({ icon: 'warning', title: 'Cảnh báo', text: 'Thời gian kết thúc phải sau thời gian hiện tại!' });
                document.getElementById("topicEndTime").focus();
                return;
            }
            const endTimeISO = endTimeDate.toISOString();
            try {
                const response = await fetch('https://localhost:7193/api/Topics'
                //const response = await fetch('https://class.criticore.edu.vn:8005/api/Topics'
                    , {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        classId: parseInt(cls.ClassId),
                        title: title,
                        description: desc,
                        type: "discussion",
                        endTime: endTimeISO,
                        createdBy: parseInt(window.currentUserId) || 1 // Sửa thành int UserId, fallback 1 nếu undefined
                    })
                });
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                await response.json(); // Confirm
                loadTopics(); // Reload từ DB
                // Reset form
                document.getElementById("topicTitle").value = "";
                document.getElementById("topicDesc").value = "";
                document.getElementById("topicEndTime").value = "";
                Swal.fire({ icon: 'success', title: 'Thành công', text: 'Topic đã được tạo!', timer: 1500 });
            } catch (error) {
                console.error('Error creating topic:', error);
                // Lỗi
                Swal.fire({ icon: 'error', title: 'Lỗi', text: 'Không thể tạo topic. Vui lòng thử lại.' });
            }
        };
    }
    // ==== GROUP MANAGEMENT ====
    const createGroupBtn = document.getElementById("createGroupBtn");
    if (createGroupBtn) {
        createGroupBtn.addEventListener("click", () => {
            if (!cls.groups) cls.groups = [];
            cls.groups.push({ members: [] });
            renderGroups();
        });
    }
    // ======= RANKING FEATURE =======
    const rankButton = document.getElementById("rankButton");
    if (rankButton) {
        rankButton.addEventListener("click", () => {
            renderRankModal();
            document.getElementById("rankModal").classList.remove("hidden");
        });
    }
    const closeRankModal = document.getElementById("closeRankModal");
    if (closeRankModal) {
        closeRankModal.addEventListener("click", () => {
            document.getElementById("rankModal").classList.add("hidden");
        });
    }
    // ==== REPORT FEATURE ====
    const reportButton = document.getElementById("reportButton");
    if (reportButton) {
        reportButton.addEventListener("click", () => {
            renderReportModal();
            document.getElementById("reportModal").classList.remove("hidden");
        });
    }
    const closeReportModal = document.getElementById("closeReportModal");
    if (closeReportModal) {
        closeReportModal.addEventListener("click", () => {
            document.getElementById("reportModal").classList.add("hidden");
        });
    }
});