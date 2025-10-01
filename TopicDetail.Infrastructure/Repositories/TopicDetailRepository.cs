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
            return await _context.Answers
                .Where(a => a.TopicId == topicId)
                .Include(a => a.User) // Join với User entity (giả sử Answer có navigation property đến User)
                .Include(a => a.Votes) // Thêm include Votes để tính rating
                .ToListAsync();
        }
        public async Task<Answer?> GetAnswerByIdAsync(int id)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Votes)
                .FirstOrDefaultAsync(a => a.AnswerId == id);
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
        // CRUD for Vote (thêm methods cho Vote)
        public async Task<IEnumerable<Vote>> GetVotesByAnswerIdAsync(int answerId)
        {
            return await _context.Votes
                .Where(v => v.AnswerId == answerId)
                .ToListAsync();
        }
        public async Task<Vote?> GetVoteByIdAsync(int id)
        {
            return await _context.Votes.FirstOrDefaultAsync(v => v.VoteId == id);
        }
        public async Task<Vote?> GetVoteByAnswerAndUserAsync(int answerId, int userId)
        {
            return await _context.Votes.FirstOrDefaultAsync(v => v.AnswerId == answerId && v.UserId == userId);
        }
        public async Task<Vote> CreateVoteAsync(Vote vote)
        {
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            return vote;
        }
        public async Task UpdateVoteAsync(Vote vote)
        {
            _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteVoteAsync(int id)
        {
            var vote = await GetVoteByIdAsync(id);
            if (vote != null)
            {
                _context.Votes.Remove(vote);
                await _context.SaveChangesAsync();
            }
        }
    }
}