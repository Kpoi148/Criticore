using Microsoft.AspNetCore.Authorization;  // Thêm
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Class.Domain.Entities;
using Front_end.Services.Interfaces;
using Front_end.Models;

namespace Front_end.Pages.Class
{
    [Authorize]  // Thêm: Tự redirect nếu không auth
    public class ClassListModel : PageModel
    {
        private readonly IClassesService _service;

        public ClassListModel(IClassesService service)
        {
            _service = service;
        }

        public List<ClassSummaryDto> Classes { get; set; } = new();
        public List<User> Students { get; set; } = new();
        public string? CurrentUserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy userId từ claims
            CurrentUserId = User.FindFirst("UserId")?.Value;
            Console.WriteLine($"UserId từ claims: {CurrentUserId}");

            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return RedirectToPage("/Signin");
            }

            // Gọi service để lấy danh sách lớp theo userId
            int userId = int.Parse(CurrentUserId);
            Classes = await _service.GetClassesByUserAsync(userId);

            // Demo danh sách Students cho chức năng tạo class (có thể load từ API sau)
            //Students = new List<User>
            //{
            //    new() { UserId = 1, FullName = "Student1" },
            //    new() { UserId = 2, FullName = "Student2" },
            //    new() { UserId = 3, FullName = "Student3" }
            //};

            return Page();
        }
    }
    
}