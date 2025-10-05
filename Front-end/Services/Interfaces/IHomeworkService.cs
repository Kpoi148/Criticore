using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface IHomeworkService
    {
        Task<List<HomeworkDto>> GetByTopicAsync(int topicId);
        Task<HomeworkDto?> GetByIdAsync(int id);
        Task<HomeworkDto?> CreateAsync(HomeworkCreateDto dto);
        Task<bool> DeleteAsync(int homeworkId);
    }
}
