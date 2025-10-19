using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.Entities;
using Homework.Domain.Repositories;

namespace Homework.Application.Services
{
    public class CopyleaksReportService
    {
        private readonly ICopyleaksReportRepository _reportRepo;

        public CopyleaksReportService(ICopyleaksReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }

        public async Task<IEnumerable<CopyleaksReport>> GetReportsByUserAsync(int userId)
        {
            return await _reportRepo.GetReportsByUserAsync(userId);
        }
    }
}
