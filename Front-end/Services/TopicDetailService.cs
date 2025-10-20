using TopicDetail.Application.DTOs;
using Front_end.Services.Interfaces;
using System.Net.Http.Json;

public class TopicDetailService : ITopicDetailService
{
    private readonly HttpClient _httpClient;
    public TopicDetailService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7134/"); // Giữ nguyên base URL
        _httpClient.BaseAddress = new Uri("https://topicdetail.criticore.edu.vn:8009/");
    }
    public async Task<IEnumerable<AnswerDto>> GetAnswersByTopicIdAsync(int topicId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<AnswerDto>>($"api/TopicDetail/topics/{topicId}/answers") ?? Enumerable.Empty<AnswerDto>();
    }
    public async Task<AnswerDto?> GetAnswerByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<AnswerDto>($"api/TopicDetail/answers/{id}");
    }
    public async Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/TopicDetail/answers", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AnswerDto>() ?? throw new Exception("Failed to read created answer");
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

    // Vote methods
    public async Task<IEnumerable<VoteDto>> GetVotesByAnswerIdAsync(int answerId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<VoteDto>>($"api/TopicDetail/answers/{answerId}/votes") ?? Enumerable.Empty<VoteDto>();
    }
    public async Task<VoteDto?> GetVoteByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<VoteDto>($"api/TopicDetail/votes/{id}");
    }
    public async Task<VoteDto> CreateOrUpdateVoteAsync(CreateVoteDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/TopicDetail/votes", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VoteDto>() ?? throw new Exception("Failed to read vote");
    }
    public async Task UpdateVoteAsync(int id, UpdateVoteDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/TopicDetail/votes/{id}", dto);
        response.EnsureSuccessStatusCode();
    }
    public async Task DeleteVoteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/TopicDetail/votes/{id}");
        response.EnsureSuccessStatusCode();
    }
}