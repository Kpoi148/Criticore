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
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly HomeworkDbContext _context;

        public SubmissionRepository(HomeworkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Submission>> GetAllAsync()
        {
            return await _context.Submissions
                .Include(s => s.Homework)
                .ToListAsync();
        }

        public async Task<IEnumerable<Submission>> GetByHomeworkIdAsync(int homeworkId)
        {
            return await _context.Submissions
                .Include(s => s.Homework)
                .Where(s => s.HomeworkId == homeworkId)
                .ToListAsync();
        }

        public async Task<Submission?> GetByIdAsync(int id)
        {
            return await _context.Submissions
                .Include(s => s.Homework)
                .FirstOrDefaultAsync(s => s.SubmissionId == id);
        }

        public async Task AddAsync(Submission submission)
        {
            await _context.Submissions.AddAsync(submission);
        }

        public async Task UpdateAsync(Submission submission)
        {
            _context.Submissions.Update(submission);
        }

        public async Task DeleteAsync(int id)
        {
            var sub = await _context.Submissions.FindAsync(id);
            if (sub != null)
                _context.Submissions.Remove(sub);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
