using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.AdminClasses
{
    public class EditModel : PageModel
    {
        private readonly IClassesService _service;
        public EditModel(IClassesService service) => _service = service;

        [BindProperty]
        public ClassDto ClassInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cls = await _service.GetByIdAsync(id);
            if (cls == null) return NotFound();

            ClassInput = cls;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _service.UpdateAsync(ClassInput.ClassId, ClassInput);
            if (!success)
            {
                ModelState.AddModelError("", "Cập nhật lớp thất bại.");
                return Page();
            }

            return RedirectToPage("Details", new { id = ClassInput.ClassId });
        }
    }
}
