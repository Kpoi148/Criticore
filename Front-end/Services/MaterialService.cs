using System.Net.Http.Headers;
using Front_end.Models;
using Front_end.Services.Interfaces;

namespace Front_end.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly HttpClient _http;
        public MaterialService(HttpClient http) => _http = http;

        public async Task<List<MaterialDto>> GetByClassIdAsync(int classId)
        {
            return await _http.GetFromJsonAsync<List<MaterialDto>>($"api/Material/class/{classId}")
                   ?? new List<MaterialDto>();
        }

        public async Task<bool> UploadAsync(IFormFile file, int classId, int uploadedBy)
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName);
            content.Add(new StringContent(classId.ToString()), "classId");
            content.Add(new StringContent(uploadedBy.ToString()), "uploadedBy");

            var res = await _http.PostAsync("api/Material/upload", content);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int materialId)
        {
            var res = await _http.DeleteAsync($"api/Material/{materialId}");
            return res.IsSuccessStatusCode;
        }
    }
}
