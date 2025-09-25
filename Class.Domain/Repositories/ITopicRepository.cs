using Class.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Class.Domain.Repositories;

public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetAllAsync();
    Task<Topic?> GetByIdAsync(int id);
    Task AddAsync(Topic topic);
    Task UpdateAsync(Topic topic);
    Task DeleteAsync(int id); 
    Task<IEnumerable<Topic>> GetAllByClassAsync(int classId);
}