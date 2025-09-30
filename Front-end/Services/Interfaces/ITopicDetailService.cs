using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface ITopicDetailService
    {
        Task<IEnumerable<AnswerDto>> GetAnswersByTopicIdAsync(int topicId);
        Task<AnswerDto?> GetAnswerByIdAsync(int id);
        Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto dto);
        Task UpdateAnswerAsync(int id, UpdateAnswerDto dto);
        Task DeleteAnswerAsync(int id);
    }
}