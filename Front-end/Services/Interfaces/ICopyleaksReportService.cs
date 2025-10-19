using Front_end.Models;

namespace Front_end.Services.Interfaces
{
    public interface ICopyleaksReportService
    {
        Task<List<CopyleaksReportDto>> GetReportsByUserAsync(int userId);
        Task<CopyleaksReportDto?> GetByIdAsync(int id);
    }
}
