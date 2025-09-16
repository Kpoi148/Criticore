using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Class.Domain.Repositories
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class.Domain.Entities.Class>> GetAllAsync();
        Task<Class.Domain.Entities.Class?> GetByIdAsync(int id);
        Task AddAsync(Class.Domain.Entities.Class cls);
        Task UpdateAsync(Class.Domain.Entities.Class cls);
        Task DeleteAsync(int id);
    }
}
