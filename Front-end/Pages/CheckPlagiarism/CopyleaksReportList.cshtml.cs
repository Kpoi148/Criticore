using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.CheckPlagiarism
{
    public class CopyleaksReportListModel : PageModel
    {

        private readonly ICopyleaksReportService _service;

        public CopyleaksReportListModel(ICopyleaksReportService service)
        {
            _service = service;
        }
            
        public List<CopyleaksReportDto> Reports { get; set; } = new();
        public string? CurrentUserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUserId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToPage("/Signin");

            int userId = int.Parse(CurrentUserId);
            Reports = await _service.GetReportsByUserAsync(userId);

            return Page();
        }
    }
}
