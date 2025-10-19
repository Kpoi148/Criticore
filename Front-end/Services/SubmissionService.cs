using Front_end.Models;
using Front_end.Services.Interfaces;

namespace Front_end.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly HttpClient _http;

        public SubmissionService(HttpClient http)
        {
            _http = http;
            //_http.BaseAddress = new Uri("https://homework.criticore.edu.vn:8007/"); // base URL của Homework API
            _http.BaseAddress = new Uri("https://localhost:7154/"); // base URL của Homework API
        }

        // Lấy danh sách submission theo homeworkId
        public async Task<List<SubmissionReadDto>> GetByHomeworkAsync(int homeworkId)
        {
            try
            {
                var response = await _http.GetAsync($"api/submission/homework/{homeworkId}");
                if (!response.IsSuccessStatusCode)
                    return new List<SubmissionReadDto>();

                var data = await response.Content.ReadFromJsonAsync<List<SubmissionReadDto>>();
                return data ?? new List<SubmissionReadDto>();
            }
            catch
            {
                return new List<SubmissionReadDto>();
            }
        }

        // Lấy submission theo homeworkId + userId
        public async Task<SubmissionReadDto?> GetByHomeworkAndUserAsync(int homeworkId, int userId)
        {
            try
            {
                var response = await _http.GetAsync($"api/submission/homework/{homeworkId}/user/{userId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<SubmissionReadDto>();
            }
            catch
            {
                return null;
            }
        }
        // Tạo mới submission
        public async Task<SubmissionReadDto?> CreateAsync(SubmissionCreateDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/submission", dto);
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<SubmissionReadDto>();
            }
            catch
            {
                return null;
            }
        }
        // Cập nhật submission
        public async Task<SubmissionReadDto?> UpdateAsync(SubmissionUpdateDto dto, int submissionId)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"api/submission/{submissionId}", dto);
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<SubmissionReadDto>();
            }
            catch
            {
                return null;
            }
        }

    }
}
