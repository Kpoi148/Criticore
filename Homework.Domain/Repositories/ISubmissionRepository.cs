using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.Entities;

namespace Homework.Domain.Repositories
{
    public interface ISubmissionRepository
    {
        Task<IEnumerable<Submission>> GetAllAsync();
        Task<IEnumerable<Submission>> GetByHomeworkIdAsync(int homeworkId);
        Task<Submission?> GetByIdAsync(int id);
        Task AddAsync(Submission submission);
        Task UpdateAsync(Submission submission);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
