using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface IMaterialService
    {
        Task<List<MaterialDto>> GetByClassIdAsync(int classId);
        Task<bool> UploadAsync(IFormFile file, int classId, int uploadedBy, int? homeworkId = null);
        Task<bool> DeleteAsync(int materialId);
        Task<List<MaterialDto>> GetByHomeworkAsync(int homeworkId);
    }
}
