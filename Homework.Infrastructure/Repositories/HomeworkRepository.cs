using Homework.Domain.DTOs;
using Homework.Domain.Entities;
using Homework.Domain.Repositories;
using Homework.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Infrastructure.Repositories
{
    public class HomeworkRepository : IHomeworkRepository
    {
        private readonly HomeworkDbContext _context; // Đổi tên DbContext nếu cần

        public HomeworkRepository(HomeworkDbContext context) { _context = context; }

        public async Task<IEnumerable<HomeWork>> GetAllAsync()
          => await _context.HomeWorks.ToListAsync();

        public async Task<HomeWork?> GetByIdAsync(int id)
        {
            // Có thể dùng Include để nạp thêm thông tin User hoặc Topic nếu cần
            return await _context.HomeWorks
                .FirstOrDefaultAsync(h => h.HomeworkId == id);
        }

        public async Task<IEnumerable<HomeWork>> GetByTopicAsync(int topicId)
        {
            return await _context.HomeWorks
                .Include(h => h.Submissions)
                .Where(h => h.TopicId == topicId)
                .ToListAsync();
        }

        public async Task AddAsync(HomeWork homework)
        {
            _context.HomeWorks.Add(homework);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HomeWork homework)
        {
            // EF Core theo dõi entity, chỉ cần SaveChanges là được, hoặc dùng Update rõ ràng
            _context.HomeWorks.Update(homework);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var homework = await _context.HomeWorks.FindAsync(id);
            if (homework != null)
            {
                _context.HomeWorks.Remove(homework);
                await _context.SaveChangesAsync();
            }
            // Tùy chọn: ném lỗi nếu không tìm thấy, hoặc để mặc định (không làm gì)
        }

        public async Task<List<DeadlineDto>> GetDeadlinesByUserAsync(int userId)
        {
            return await _context.HomeWorks
                .Include(h => h.Topic) // Include để lấy Topic
                .ThenInclude(t => t.Class) // Include để lấy Class từ Topic
                .Where(h => h.DueDate != null &&
                            _context.ClassMembers.Any(cm => cm.UserId == userId && cm.ClassId == h.Topic.ClassId))
                .Select(h => new DeadlineDto
                {
                    HomeworkId = h.HomeworkId,
                    Title = h.Title ?? string.Empty,
                    TopicTitle = h.Topic.Title ?? string.Empty,
                    ClassName = h.Topic.Class.ClassName ?? string.Empty,
                    DueDate = h.DueDate,
                    Status = h.Status
                })
                .ToListAsync();
        }
    }
}
