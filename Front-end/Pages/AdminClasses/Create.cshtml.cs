using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.AdminClasses
{
    public class CreateModel : PageModel
    {
        private readonly IClassesService _service;
        public CreateModel(IClassesService service) => _service = service;

        [BindProperty]
        public ClassCreateDto ClassInput { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            //// Lấy user id từ session (hoặc claims). Nếu null => yêu cầu login
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null || userId == 0)
            //{
            //    TempData["Error"] = "Bạn cần đăng nhập để tạo lớp.";
            //    return RedirectToPage("/Account/Login");
            //}

            // Gán CreatedBy an toàn ở server-side
            //ClassInput.CreatedBy = userId.Value;
            ClassInput.CreatedBy = 1;

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
