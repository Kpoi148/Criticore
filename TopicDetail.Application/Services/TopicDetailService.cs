using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TopicDetail.Application.DTOs;
using TopicDetail.Domain.Models;
using TopicDetail.Domain.Repositories;

namespace TopicDetail.Application.Services
{
    public class TopicDetailService
    {
        private readonly ITopicDetailRepository _repository;
        private readonly IMapper _mapper;

        public TopicDetailService(ITopicDetailRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // CRUD for Answer
        public async Task<IEnumerable<AnswerDto>> GetAnswersByTopicIdAsync(int topicId)
        {
            var answers = await _repository.GetAnswersByTopicIdAsync(topicId);
            return _mapper.Map<IEnumerable<AnswerDto>>(answers);
        }

        public async Task<AnswerDto?> GetAnswerByIdAsync(int id)
        {
            var answer = await _repository.GetAnswerByIdAsync(id);
            return _mapper.Map<AnswerDto>(answer);
        }

        public async Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto dto)
        {
            var answer = _mapper.Map<Answer>(dto);
            answer.CreatedAt = DateTime.UtcNow;
            var created = await _repository.CreateAnswerAsync(answer);
            return _mapper.Map<AnswerDto>(created);
        }

        public async Task UpdateAnswerAsync(int id, UpdateAnswerDto dto)
        {
            var answer = await _repository.GetAnswerByIdAsync(id);
            if (answer != null)
            {
                _mapper.Map(dto, answer);
                answer.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAnswerAsync(answer);
            }
        }

        public async Task DeleteAnswerAsync(int id)
        {
            await _repository.DeleteAnswerAsync(id);
        }
    }
}