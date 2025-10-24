using Microsoft.AspNetCore.Authorization;  // Thêm
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Class.Domain.Entities;
using Front_end.Services.Interfaces;
using Front_end.Models;
using Class.Application.Services;

namespace Front_end.Pages.Class
{
    //[Authorize]  // Thêm: Tự redirect nếu không auth
    public class ClassListModel : PageModel
    {
        private readonly IClassesService _service;
        private readonly IUsersService _userService;

        public ClassListModel(IClassesService service, IUsersService userService)
        {
            _service = service;
            _userService = userService;
        }

        public List<ClassSummaryDto> Classes { get; set; } = new();
        public Dictionary<int, int> TopicCounts { get; set; } = new();
        public List<User> Students { get; set; } = new();
        public string? CurrentUserId { get; set; }
        [BindProperty]
        public string JoinCode { get; set; } = string.Empty;
        public string? UserRole { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy userId từ claims
            CurrentUserId = User.FindFirst("UserId")?.Value;
            Console.WriteLine($"UserId từ claims: {CurrentUserId}");
            UserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return RedirectToPage("/Signin");
            }

            // Gọi service để lấy danh sách lớp theo userId
            int userId = int.Parse(CurrentUserId);
            Classes = await _service.GetClassesByUserAsync(userId);
            // Lấy số lượng topic trong mỗi lớp học
            foreach (var cls in Classes)
            {
                var count = await _service.GetTopicCountByClassAsync(cls.ClassId);
                Console.WriteLine($"Class {cls.ClassId} - {cls.ClassName} has {count} topics");
                TopicCounts[cls.ClassId] = count;
            }

            return Page();
        }
        public async Task<IActionResult> OnPostCreateAsync(ClassCreateDto ClassInput)
        {
            // Lấy UserId của giáo viên hiện tại từ claims
            var currentUserId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToPage("/Signin");
            }

            // Gán các giá trị bắt buộc mà client không nhập
            ClassInput.CreatedBy = int.Parse(currentUserId);
            ClassInput.TeacherId = int.Parse(currentUserId); // vì giáo viên hiện tại là người tạo lớp

            // Gọi service để tạo lớp
            var createdId = await _service.CreateAsync(ClassInput);

            if (createdId == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to create the class. Please try again later.");
                return Page();
            }

            // Redirect hoặc return JSON nếu gọi AJAX
            return RedirectToPage("/Class/ClassList");
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
            var currentUserId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return RedirectToPage("/Signin");
            int userId2 = int.Parse(currentUserId);

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