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
        [BindProperty]
        public string JoinCode { get; set; } = string.Empty;
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

            return Page();
        }
        public async Task<IActionResult> OnPostJoinAsync()
        {
            if (string.IsNullOrWhiteSpace(JoinCode))
            {
                ModelState.AddModelError("", "Class code cannot be empty.");

                // Load lại danh sách lớp
                var userIdStr = User.FindFirst("UserId")?.Value;
                if (!string.IsNullOrEmpty(userIdStr))
                {
                    int userId = int.Parse(userIdStr);
                    Classes = await _service.GetClassesByUserAsync(userId);
                }

                return Page();
            }

            // Lấy userId từ claims
            var userIdStr2 = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdStr2)) return RedirectToPage("/Signin");
            int userId2 = int.Parse(userIdStr2);

            // Gọi service join
            var cls = await _service.JoinByCodeAsync(JoinCode, userId2);
            if (cls == null)
            {
                ModelState.AddModelError("", "Invalid or expired class code.");

                // Load lại danh sách lớp
                Classes = await _service.GetClassesByUserAsync(userId2);

                return Page();
            }

            // Redirect sang chi tiết lớp
            return RedirectToPage("/Class/ClassDetail", new { id = cls.ClassId });
        }
    }
    
}