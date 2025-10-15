using Front_end.Models;
using Front_end.Services.Interfaces;

namespace Front_end.Services
{
    public class TopicService : ITopicService
    {
        private readonly HttpClient _httpClient;

        public TopicService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7193/"); // Thay bằng base URL API, ví dụ: https://localhost:port/
            //_httpClient.BaseAddress = new Uri("https://class.criticore.edu.vn:8005/");
        }

        public async Task<List<TopicDto>> GetAllByClassAsync(int classId)
        {
            return await _httpClient.GetFromJsonAsync<List<TopicDto>>($"api/Topics/byclass/{classId}");
        }

        public async Task<TopicDto> CreateAsync(CreateTopicDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Topics", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TopicDto>();
        }

        public async Task<TopicDto?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TopicDto>($"api/Topics/{id}");
        }

        // Implement các method khác tương tự nếu cần
    }
}
