using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims; // Thêm thư viện này

namespace Front_end.Pages.Payment
{
    public class CreateModel : PageModel
    {
        private readonly IOrderService _orderService;
        // Thêm thuộc tính để lưu trữ thông báo từ backend
        [TempData] // Dùng TempData để giữ thông báo sau khi Redirect hoặc Post
        public string ErrorMessage { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        public CreateModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [BindProperty]
        public CreateOrderRequest Input { get; set; }

        public void OnGet()
        {
            // Không cần làm gì nhiều ở đây
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Kiểm tra UserId
            // Lấy userId từ claims
            var CurrentUserId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return RedirectToPage("/Signin");
            }

            // Gọi service để lấy danh sách lớp theo userId
            int userId = int.Parse(CurrentUserId);
            Input.UserId = userId;

            // 2. Kiểm tra tính hợp lệ của Model (packageName, Price, Duration, UserId)
            if (!ModelState.IsValid)
                return Page();

            // 3. Gọi Service tạo đơn hàng
            var success = await _orderService.CreateOrderAndGetMessageAsync(Input);

            if (string.IsNullOrEmpty(success))
            {
                // TẠO ĐƠN HÀNG THÀNH CÔNG
                SuccessMessage = "Order created successfully! Please proceed to the payment guide.";
                // Chuyển hướng đến trang hướng dẫn thanh toán
                return RedirectToPage("/Payment/PaymentGuide");
            }
            else
            {
                // TẠO ĐƠN HÀNG THẤT BẠI (Backend trả về thông báo lỗi)

                // Trường hợp đặc biệt: Nếu lỗi là "Current plan still active until..."
                if (success.Contains("Current plan still active"))
                {
                    ErrorMessage = success;
                }
                else
                {
                    // Lỗi chung khác
                    ErrorMessage = $"Order creation failed: {success}";
                }

                // Giữ nguyên trên trang để hiển thị ErrorMessage
                return Page();
            }
        }
    }
}