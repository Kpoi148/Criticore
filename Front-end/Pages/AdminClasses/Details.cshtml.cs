using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.AdminClasses
{
    public class DetailsModel : PageModel
    {
        private readonly IClassesService _service;
        public DetailsModel(IClassesService service) => _service = service;

        public ClassDto? ClassDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Index"); // Không phải admin => quay về trang chủ
            }

            ClassDetail = await _service.GetByIdAsync(id);
            if (ClassDetail == null)
            {
                return NotFound();
            }
            return Page();

        }
    }
}
