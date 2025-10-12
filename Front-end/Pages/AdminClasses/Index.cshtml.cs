using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.AdminClasses
{
    public class IndexModel : PageModel
    {
        private readonly IClassesService _service;
        public IndexModel(IClassesService service) => _service = service;

        public List<ClassDto> Classes { get; set; } = new();

        // Kiểm tra role khi load trang
        public async Task<IActionResult> OnGetAsync()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Index"); // Không phải admin => quay về trang chủ
            }

            Classes = await _service.GetAllAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}
