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
        // Manual map CreateTopicDto sang Topic entity
        var entity = new Topic
        {
            ClassId = dto.ClassId,
            Title = dto.Title,
            Description = dto.Description,
            Type = dto.Type,
            EndTime = dto.EndTime,
            CreatedBy = dto.CreatedBy
            // CreatedAt và UpdatedAt sẽ được set trong repository
        };

        await _repository.AddAsync(entity);

        // Reload entity với include để lấy CreatedByUser
        var fullEntity = await _repository.GetByIdAsync(entity.TopicId);

        if (fullEntity == null)
        {
            throw new Exception("Topic not found after creation.");
        }

        // Manual map Topic entity sang TopicDto
        var topicDto = new TopicDto
        {
            TopicId = fullEntity.TopicId,
            ClassId = fullEntity.ClassId,
            Title = fullEntity.Title,
            Description = fullEntity.Description,
            Type = fullEntity.Type,
            EndTime = fullEntity.EndTime,
            CreatedBy = fullEntity.CreatedBy,
            CreatedByName = fullEntity.CreatedByNavigation?.FullName ?? "Unknown", // Populate tên từ navigation property
            CreatedAt = fullEntity.CreatedAt,
            UpdatedAt = fullEntity.UpdatedAt
        };

        return topicDto;
    }
    public async Task<IEnumerable<TopicDto>> GetAllByClassAsync(int classId)
    {
        var entities = await _repository.GetAllByClassAsync(classId);
        var dtos = new List<TopicDto>();

        foreach (var entity in entities)
        {
            dtos.Add(new TopicDto
            {
                TopicId = entity.TopicId,
                ClassId = entity.ClassId,
                Title = entity.Title,
                Description = entity.Description,
                Type = entity.Type,
                EndTime = entity.EndTime,
                CreatedBy = entity.CreatedBy,
                CreatedByName = entity.CreatedByNavigation?.FullName ?? "Unknown",
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            });
        }

        return dtos;
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