using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Domain.Repositories
{
    public interface IHomeworkRepository
    {
        Task<IEnumerable<Homework.Domain.Entities.HomeWork>> GetAllAsync();
        Task<Homework.Domain.Entities.HomeWork?> GetByIdAsync(int id);
        Task<IEnumerable<Homework.Domain.Entities.HomeWork>> GetByTopicAsync(int topicId); // Lấy theo Topic
        Task AddAsync(Homework.Domain.Entities.HomeWork homework);
        Task UpdateAsync(Homework.Domain.Entities.HomeWork homework);
        Task DeleteAsync(int id);
    }
}
