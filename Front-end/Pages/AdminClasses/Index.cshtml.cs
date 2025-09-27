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

        public async Task OnGetAsync()
        {
            Classes = await _service.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}
