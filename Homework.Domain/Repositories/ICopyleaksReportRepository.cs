using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.Entities;

namespace Homework.Domain.Repositories
{
    public interface ICopyleaksReportRepository
    {
        Task AddReportAsync(CopyleaksReport report);
        Task SaveChangesAsync();
        Task<IEnumerable<CopyleaksReport>> GetReportsByUserAsync(int userId);
    }
}
