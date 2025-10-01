using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.Repositories;
using Homework.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Homework.Infrastructure.Repositories
{
    public class HomeworkRepository : IHomeworkRepository
    {
        private readonly HomeworkDbContext _context; // Đổi tên DbContext nếu cần

        public HomeworkRepository(HomeworkDbContext context) { _context = context; }

        public async Task<IEnumerable<Homework.Domain.Entities.HomeWork>> GetAllAsync()
          => await _context.HomeWorks.ToListAsync();

        public async Task<Homework.Domain.Entities.HomeWork?> GetByIdAsync(int id)
        {
            // Có thể dùng Include để nạp thêm thông tin User hoặc Topic nếu cần
            return await _context.HomeWorks
                .FirstOrDefaultAsync(h => h.HomeworkId == id);
        }

        public async Task<IEnumerable<Homework.Domain.Entities.HomeWork>> GetByTopicAsync(int topicId)
        {
            return await _context.HomeWorks
                .Where(h => h.TopicId == topicId)
                .ToListAsync();
        }

        public async Task AddAsync(Homework.Domain.Entities.HomeWork homework)
        {
            _context.HomeWorks.Add(homework);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Homework.Domain.Entities.HomeWork homework)
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
    }
}
