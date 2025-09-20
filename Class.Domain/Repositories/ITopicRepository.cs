using System.Collections.Generic;
using System.Threading.Tasks;
using Class.Domain.Entities;

namespace Class.Domain.Repositories;

public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetAllAsync();
    Task<Topic?> GetByIdAsync(int id);
    Task AddAsync(Topic topic);
    Task UpdateAsync(Topic topic);
    Task DeleteAsync(int id);
}