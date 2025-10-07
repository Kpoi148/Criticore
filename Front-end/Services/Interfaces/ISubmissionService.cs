using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<List<SubmissionReadDto>> GetByHomeworkAsync(int homeworkId);
        Task<SubmissionReadDto?> CreateAsync(SubmissionCreateDto dto);
        Task<SubmissionReadDto?> GetByHomeworkAndUserAsync(int homeworkId, int userId);
        Task<SubmissionReadDto?> UpdateAsync(SubmissionUpdateDto dto, int submissionId);
    }
}
