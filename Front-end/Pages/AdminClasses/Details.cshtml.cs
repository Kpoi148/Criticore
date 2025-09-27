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

        public async Task OnGetAsync(int id)
        {
            ClassDetail = await _service.GetByIdAsync(id);
        }
    }
}
