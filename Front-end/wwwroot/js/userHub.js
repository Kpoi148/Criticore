//// --- Lấy userId hiện tại từ session/token ---
//const token = sessionStorage.getItem("authToken");
//let myUserId = null;
//if (token) {
//    try {
//        const payload = JSON.parse(atob(token.split('.')[1]));
//        myUserId = parseInt(payload.UserId);
//    } catch (err) {
//        console.error("Lỗi decode token", err);
//    }
//}

//// --- Khởi tạo SignalR connection ---
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("https://localhost:7281/userHub") // URL API của TopicDetail (thay bằng production URL)
//    .withAutomaticReconnect()
//    .build();

//// --- Event: user bị ban ---
//connection.on("UserBanned", data => {
//    if (data.UserId === myUserId) {
//        alert("⚠️ Bạn đã bị admin ban!");
//        window.location.href = "/SignIn";
//    }
//});

//// --- Event: cập nhật status trên bảng admin ---
//connection.on("UserStatusChanged", data => {
//    const row = document.querySelector(`tr[data-user-id='${data.UserId}']`);
//    if (!row) return;
//    const statusBadge = row.querySelector(".status-badge");
//    if (statusBadge) {
//        statusBadge.textContent = data.Status;
//        statusBadge.className = "badge " +
//            (data.Status === "Active" ? "bg-success fw-semibold status-badge" : "bg-secondary fw-semibold status-badge");
//    }
//});

//// --- Event: cập nhật role trên bảng admin ---
//connection.on("UserRoleChanged", data => {
//    const row = document.querySelector(`tr[data-user-id='${data.UserId}']`);
//    if (!row) return;
//    const roleBadge = row.querySelector(".role-badge");
//    if (roleBadge) {
//        roleBadge.textContent = data.Role;
//        roleBadge.className = "badge " +
//            (data.Role === "Admin" ? "bg-danger" : (data.Role === "Teacher" ? "bg-warning text-dark" : "bg-info")) +
//            " fw-semibold role-badge";
//    }
//});

//// --- Bắt đầu connection ---
//connection.start()
//    .then(() => console.log("Connected to UserHub"))
//    .catch(err => console.error("SignalR connection error:", err));
