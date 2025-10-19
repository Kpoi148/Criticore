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

        public IActionResult OnGet()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out _))
                return RedirectToPage("/Signin");

            return Page();
        }

        public async Task<IActionResult> OnPostCheckAsync()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdString, out int userId))
                return RedirectToPage("/Signin");
            
            if (FileToCheck == null || FileToCheck.Length == 0)
            {
                ScanResultMessage = "Please select a file to check.";
                return Page();
            }

            try
            {
                var scanId = await _copyleaksService.SubmitFileForScanAsync(FileToCheck, userId);
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
