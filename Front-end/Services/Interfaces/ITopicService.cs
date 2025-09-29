using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface ITopicService
    {
        Task<List<TopicDto>> GetAllByClassAsync(int classId);
        Task<TopicDto> CreateAsync(CreateTopicDto dto);
        Task<TopicDto> GetByIdAsync(int id);
    }
}
