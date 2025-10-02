using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Front_end.Pages.AdminClasses
{
    public class CreateModel : PageModel
    {
        private readonly IClassesService _service;
        private readonly IUsersService _userService; // service để gọi api teachers

        public CreateModel(IClassesService service, IUsersService userService)
        {
            _service = service;
            _userService = userService;
        }
        [BindProperty]
        public ClassCreateDto ClassInput { get; set; } = new();
        public string? CurrentUserId { get; set; }
        // Danh sách giáo viên để render ra dropdown
        public List<SelectListItem> Teachers { get; set; } = new();

        public async Task OnGetAsync()
        {
            // gọi API lấy teachers
            var teachers = await _userService.GetTeachersAsync();
            Teachers = teachers.Select(t => new SelectListItem
            {
                Value = t.UserId.ToString(),
                Text = t.FullName
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            // Lấy userId từ claims
            CurrentUserId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return RedirectToPage("/Signin");
            }
            int userId = int.Parse(CurrentUserId);

            ClassInput.CreatedBy = userId;

            // Gọi service để tạo
            var createdId = await _service.CreateAsync(ClassInput);

            if (createdId == null)
            {
                ModelState.AddModelError(string.Empty, "Tạo lớp thất bại. Xin thử lại.");
                return Page();
            }

            return RedirectToPage("Details", new { id = createdId.Value });
        }
    }
}
