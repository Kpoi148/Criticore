using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.Entities;
using Homework.Domain.Repositories;
using Homework.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Homework.Infrastructure.Repositories
{
    public class CopyleaksReportRepository : ICopyleaksReportRepository
    {
        private readonly HomeworkDbContext _context;
        public CopyleaksReportRepository(HomeworkDbContext context)
        {
            _context = context;
        }

        public async Task AddReportAsync(CopyleaksReport report)
        {
            await _context.CopyleaksReports.AddAsync(report);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CopyleaksReport>> GetReportsByUserAsync(int userId)
        {
            return await _context.CopyleaksReports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}

