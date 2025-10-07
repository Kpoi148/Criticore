using System.Net.Http.Headers;
using Front_end.Services.Interfaces;

namespace Front_end.Services
{
    public class SubmissionFileService : ISubmissionFileService
    {
        private readonly HttpClient _http;
        public SubmissionFileService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://localhost:7179/"); // base URL của Homework API
        }

        public async Task<string?> UploadAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName);

            var res = await _http.PostAsync("api/SubmissionFile/upload", content);
            if (!res.IsSuccessStatusCode) return null;

            // Parse JSON trả về: { message, fileUrl }
            var json = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return json != null && json.TryGetValue("fileUrl", out var url) ? url : null;
        }
    }
}
