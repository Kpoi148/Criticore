using Front_end.Models;
using Front_end.Services.Interfaces;

public class TopicDetailService : ITopicDetailService
{
    private readonly HttpClient _httpClient;

    public TopicDetailService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7134/"); // Giữ nguyên base URL
    }

    public async Task<IEnumerable<AnswerDto>> GetAnswersByTopicIdAsync(int topicId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<AnswerDto>>($"api/TopicDetail/topics/{topicId}/answers");
    }

    public async Task<AnswerDto?> GetAnswerByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<AnswerDto>($"api/TopicDetail/answers/{id}");
    }

    public async Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/TopicDetail/answers", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AnswerDto>();
    }

    public async Task UpdateAnswerAsync(int id, UpdateAnswerDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/TopicDetail/answers/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAnswerAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/TopicDetail/answers/{id}");
        response.EnsureSuccessStatusCode();
    }
}