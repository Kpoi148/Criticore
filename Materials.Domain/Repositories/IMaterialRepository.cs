using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Material.Domain.Repositories
{
    public interface IMaterialRepository
    {
        Task<Domain.Entities.Material> AddAsync(Domain.Entities.Material material);
        Task<Domain.Entities.Material?> GetByIdAsync(int id);
        Task<IEnumerable<Domain.Entities.Material>> GetByClassIdAsync(int classId);
        Task<IEnumerable<Domain.Entities.Material>> GetAllAsync();
        Task DeleteAsync(int id);
    }
}
