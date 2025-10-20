using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Front_end.Pages.Calendar
{
    public class MyCalenderModel : PageModel
    {
        private readonly IUsersService _usersService;

        public int UserId { get; set; }
        public string UserName { get; set; } = "Người dùng không xác định";

        public MyCalenderModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task OnGetAsync()
        {
            // Lấy UserId từ token
            var userIdClaim = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserId = int.Parse(userIdClaim);

            // 🔥 Gọi API lấy thông tin user theo ID
            var user = await _usersService.GetByIdAsync(UserId);           
                UserName = user.FullName ?? $"User {UserId}";          
            Console.WriteLine($"Username: {UserName}");
        }
    }
}
