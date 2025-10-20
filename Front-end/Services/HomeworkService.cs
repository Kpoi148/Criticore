using Front_end.Models;
using Front_end.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace Front_end.Services
{
    public class HomeworkService : IHomeworkService
    {
        private readonly HttpClient _http;

        public HomeworkService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://homework.criticore.edu.vn:8007/"); // base URL của Homework API
            //_http.BaseAddress = new Uri("https://localhost:7154/"); // base URL của Homework API
        }

        // Lấy danh sách homework theo TopicId (khớp với backend route)
        public async Task<List<HomeworkDto>> GetByTopicAsync(int topicId)
        {
            return await _http.GetFromJsonAsync<List<HomeworkDto>>($"api/topics/{topicId}/homeworks")
                   ?? new List<HomeworkDto>();
        }

        // Lấy chi tiết homework theo ID
        public async Task<HomeworkDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<HomeworkDto>($"api/homeworks/{id}");
        }

        // Tạo mới homework
        public async Task<HomeworkDto?> CreateAsync(HomeworkCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/homeworks", dto);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<HomeworkDto>();
        }

        // Xóa homework
        public async Task<bool> DeleteAsync(int homeworkId)
        {
            var response = await _http.DeleteAsync($"api/homeworks/{homeworkId}");
            return response.IsSuccessStatusCode;
        }
    }
}

