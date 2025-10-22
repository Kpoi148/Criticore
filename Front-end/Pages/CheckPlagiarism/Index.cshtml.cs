using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Linq; // Thêm namespace này cho LINQ (Contains)

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

            // 1. KIỂM TRA TỒN TẠI
            if (FileToCheck == null || FileToCheck.Length == 0)
            {
                ScanResultMessage = "Error: Please select a file to check.";
                return Page();
            }

            // --- CÁC BƯỚC XÁC THỰC QUAN TRỌNG ĐỂ XỬ LÝ DỮ LIỆU KHÔNG HỢP LỆ ---

            var extension = Path.GetExtension(FileToCheck.FileName).ToLowerInvariant();
            var permittedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };

            // 2. KIỂM TRA PHẦN MỞ RỘNG FILE
            if (!permittedExtensions.Contains(extension))
            {
                ScanResultMessage = "Error: Invalid file type. Only .pdf, .doc, .docx, and .txt files are allowed.";
                return Page();
            }

            // 3. KIỂM TRA NỘI DUNG (Content Inspection)
            // Ngăn chặn JSON data giả dạng file .txt
            if (extension == ".txt")
            {
                // Đọc một phần nhỏ của file để phát hiện cấu trúc JSON (bắt đầu bằng { hoặc [)
                using (var reader = new StreamReader(FileToCheck.OpenReadStream()))
                {
                    // Đọc tối đa 500 ký tự đầu tiên
                    var startOfContent = new char[500];
                    await reader.ReadAsync(startOfContent, 0, startOfContent.Length);
                    var contentString = new string(startOfContent).Trim();

                    // Kiểm tra xem nội dung có vẻ là JSON/Array không
                    if (contentString.StartsWith("{") || contentString.StartsWith("["))
                    {
                        ScanResultMessage = "Error: File content appears to be structured data (JSON), not a plain text document.";
                        // Không cần đặt lại stream vì chúng ta sẽ trả về Page() ngay lập tức
                        return Page();
                    }

                    // Đặt con trỏ stream về đầu trước khi chuyển cho service
                    FileToCheck.OpenReadStream().Seek(0, SeekOrigin.Begin);
                }
            }
            // Lưu ý: Đối với các file nhị phân như .pdf, .docx, việc kiểm tra nội dung phức tạp hơn (cần kiểm tra header magic bytes),
            // nhưng kiểm tra phần mở rộng file và MIME type (ASP.NET Core đã xử lý một phần) là đủ cho hầu hết các trường hợp.

            // ----------------------------------------------------------------------

            try
            {
                var scanId = await _copyleaksService.SubmitFileForScanAsync(FileToCheck, userId);
                ScanResultMessage = $"File sent to Copyleaks successfully! Scan ID: {scanId}";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết hơn trong môi trường thực tế
                ScanResultMessage = $"Error when sending file to Copyleaks: {ex.Message}";
            }

            return Page();
        }
    }
}