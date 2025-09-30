using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopicDetail.Domain.Models;
using TopicDetail.Domain.Repositories;
using TopicDetail.Infrastructure.Models;

namespace TopicDetail.Infrastructure.Repositories
{
    public class TopicDetailRepository : ITopicDetailRepository
    {
        private readonly TopicDetailDbContext _context; // Thay bằng DbContext thực tế

        public TopicDetailRepository(TopicDetailDbContext context)
        {
            _context = context;
        }

        // CRUD for Answer
        public async Task<IEnumerable<Answer>> GetAnswersByTopicIdAsync(int topicId)
        {
            return await _context.Answers.Where(a => a.TopicId == topicId).ToListAsync();
        }

        public async Task<Answer?> GetAnswerByIdAsync(int id)
        {
            return await _context.Answers.FirstOrDefaultAsync(a => a.AnswerId == id);
        }

        public async Task<Answer> CreateAnswerAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswerAsync(int id)
        {
            var answer = await GetAnswerByIdAsync(id);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
            }
        }
    }
}