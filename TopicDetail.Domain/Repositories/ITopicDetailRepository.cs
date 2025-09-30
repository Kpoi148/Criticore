using System.Collections.Generic;
using System.Threading.Tasks;
using TopicDetail.Domain.Models;

namespace TopicDetail.Domain.Repositories
{
    public interface ITopicDetailRepository
    {
        // CRUD for Answer
        Task<IEnumerable<Answer>> GetAnswersByTopicIdAsync(int topicId);
        Task<Answer?> GetAnswerByIdAsync(int id);
        Task<Answer> CreateAnswerAsync(Answer answer);
        Task UpdateAnswerAsync(Answer answer);
        Task DeleteAnswerAsync(int id);
    }
}