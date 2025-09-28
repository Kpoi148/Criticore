using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface IClassesService
    {
        Task<List<ClassDto>> GetAllAsync();
        Task<ClassDto?> GetByIdAsync(int id);
        Task<int?> CreateAsync(ClassCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(int id, ClassDto dto);
        Task<List<ClassSummaryDto>> GetClassesByUserAsync(int userId);
    }
}
