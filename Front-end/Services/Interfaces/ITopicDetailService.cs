using TopicDetail.Application.DTOs;
namespace Front_end.Services.Interfaces
{
    public interface ITopicDetailService
    {
        Task<IEnumerable<AnswerDto>> GetAnswersByTopicIdAsync(int topicId);
        Task<AnswerDto?> GetAnswerByIdAsync(int id);
        Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto dto);
        Task UpdateAnswerAsync(int id, UpdateAnswerDto dto);
        Task DeleteAnswerAsync(int id);

        // Vote methods
        Task<IEnumerable<VoteDto>> GetVotesByAnswerIdAsync(int answerId);
        Task<VoteDto?> GetVoteByIdAsync(int id);
        Task<VoteDto> CreateOrUpdateVoteAsync(CreateVoteDto dto);
        Task UpdateVoteAsync(int id, UpdateVoteDto dto);
        Task DeleteVoteAsync(int id);
    }
}