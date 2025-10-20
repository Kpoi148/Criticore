using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace Front_end.Services
{
    public class CopyleaksService : ICopyleaksService
    {
        private readonly HttpClient _http;

        public CopyleaksService(HttpClient http)
        {
            _http = http;
            //_http.BaseAddress = new Uri("https://homework.criticore.edu.vn:8007/"); // base URL của Homework API
            _http.BaseAddress = new Uri("https://localhost:7154/"); // base URL của Homework API
        }
        public async Task<string?> SubmitFileForScanAsync(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return null;

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "File", file.FileName);
            content.Add(new StringContent(userId.ToString()), "UserId");

            var response = await _http.PostAsync("api/copyleaks/check-file", content);
            if (!response.IsSuccessStatusCode) return null;

            // backend trả về { message = "...", scanId = "..." }
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result != null && result.TryGetValue("scanId", out var scanId))
                return scanId;

            return null;
        }
    }
}
