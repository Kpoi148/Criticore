using Class.Domain.Entities;
using Class.Domain.Repositories;
using Class.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Class.Infrastructure.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly ClassDbContext _context; // Thay bằng DbContext của bạn
    public TopicRepository(ClassDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Topic>> GetAllAsync()
    {
        return await _context.Topics
            .Include(t => t.CreatedByNavigation) // Include navigation để fetch user nếu cần tên
            .ToListAsync();
    }

    public async Task<Topic?> GetByIdAsync(int id)
    {
        return await _context.Topics
            .Include(t => t.CreatedByNavigation) // Include để fetch user
            .FirstOrDefaultAsync(t => t.TopicId == id);
    }

    public async Task AddAsync(Topic topic)
    {
        topic.CreatedAt = DateTime.UtcNow;
        topic.UpdatedAt = DateTime.UtcNow; // Thêm UpdatedAt để nhất quán
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Topic topic)
    {
        topic.UpdatedAt = DateTime.UtcNow;
        _context.Topics.Update(topic);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var topic = await GetByIdAsync(id);
        if (topic != null)
        {
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }
    }

    // Trong TopicRepository.cs (EF Core)
    public async Task<IEnumerable<Topic>> GetAllByClassAsync(int classId)
    {
        return await _context.Topics
            .Where(t => t.ClassId == classId)
            .Include(t => t.CreatedByNavigation) // Include để fetch user name ở service
            .ToListAsync();
    }
}