using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.CheckPlagiarism
{
    public class IndexModel : PageModel
    {
        private readonly ICopyleaksService _copyleaksService;

        public IndexModel(ICopyleaksService copyleaksService)
        {
            _copyleaksService = copyleaksService;
        }

        [BindProperty]
        public IFormFile? FileToCheck { get; set; }

        public string? ScanResultMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostCheckAsync()
        {
            if (FileToCheck == null || FileToCheck.Length == 0)
            {
                ScanResultMessage = "Please select a file to check.";
                return Page();
            }

            try
            {
                var scanId = await _copyleaksService.SubmitFileForScanAsync(FileToCheck);
                ScanResultMessage = $"File sent to Copyleaks successfully! Scan ID: {scanId}";
            }
            catch (Exception ex)
            {
                ScanResultMessage = $"Error when sending file to Copyleaks: {ex.Message}";
            }

            return Page();
        }
    }
}
