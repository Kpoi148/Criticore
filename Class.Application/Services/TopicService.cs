using System.Collections.Generic;
using System.Threading.Tasks;
using Class.Domain.DTOs;
using Class.Domain.Entities;
using Class.Domain.Repositories;

namespace Class.Application.Services;

public class TopicService
{
    private readonly ITopicRepository _repository;

    public TopicService(ITopicRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TopicDto>> GetAllAsync()
    {
        var topics = await _repository.GetAllAsync();
        return topics.Select(t => new TopicDto
        {
            TopicId = t.TopicId,
            ClassId = t.ClassId,
            Title = t.Title,
            Description = t.Description,
            Type = t.Type,
            EndTime = t.EndTime,
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        });
    }

    public async Task<TopicDto?> GetByIdAsync(int id)
    {
        var topic = await _repository.GetByIdAsync(id);
        return topic == null ? null : new TopicDto
        {
            TopicId = topic.TopicId,
            ClassId = topic.ClassId,
            Title = topic.Title,
            Description = topic.Description,
            Type = topic.Type,
            EndTime = topic.EndTime,
            CreatedBy = topic.CreatedBy,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt
        };
    }

    public async Task<TopicDto> AddAsync(CreateTopicDto dto)
    {
        var topic = new Topic
        {
            ClassId = dto.ClassId,
            Title = dto.Title,
            Description = dto.Description,
            Type = dto.Type,
            EndTime = dto.EndTime,
            CreatedBy = dto.CreatedBy
        };
        await _repository.AddAsync(topic);
        return new TopicDto
        {
            TopicId = topic.TopicId,  // ID tự động sinh sau khi add
            ClassId = topic.ClassId,
            Title = topic.Title,
            Description = topic.Description,
            Type = topic.Type,
            EndTime = topic.EndTime,
            CreatedBy = topic.CreatedBy,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt
        };
    }
    public async Task<IEnumerable<TopicDto>> GetAllByClassAsync(int classId)
    {
        var topics = await _repository.GetAllByClassAsync(classId);
        return topics.Select(t => new TopicDto
        {
            TopicId = t.TopicId,
            ClassId = t.ClassId,
            Title = t.Title,
            Description = t.Description,
            Type = t.Type,
            EndTime = t.EndTime,
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        });
    }

    public async Task UpdateAsync(int id, UpdateTopicDto dto)
    {
        var topic = await _repository.GetByIdAsync(id);
        if (topic == null) throw new Exception("Topic not found");

        topic.Title = dto.Title;
        topic.Description = dto.Description;
        topic.Type = dto.Type;
        topic.EndTime = dto.EndTime;
        await _repository.UpdateAsync(topic);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}