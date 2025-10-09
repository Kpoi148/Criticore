namespace Front_end.Services.Interfaces
{
    public interface ICopyleaksService
    {
        /// <summary>
        /// Gửi file lên backend để scan Copyleaks
        /// </summary>
        /// <param name="file">File cần scan</param>
        /// <returns>ScanId hoặc null nếu thất bại</returns>
        Task<string?> SubmitFileForScanAsync(IFormFile file);
    }
}
