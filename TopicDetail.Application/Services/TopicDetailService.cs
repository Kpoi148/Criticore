using System.Collections.Generic;
using System.Linq;
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
            var dtos = _mapper.Map<IEnumerable<AnswerDto>>(answers);
            foreach (var dto in dtos)
            {
                // Sử dụng property computed Rating từ entity (đã filter VoteType == "Rating")
                var answer = answers.First(a => a.AnswerId == dto.AnswerId);
                dto.Rating = answer.Rating;
                dto.VoteCount = answer.VoteCount; // Mới: Map VoteCount
            }
            return dtos;
        }

        public async Task<AnswerDto?> GetAnswerByIdAsync(int id)
        {
            var answer = await _repository.GetAnswerByIdAsync(id);
            if (answer == null) return null;
            var dto = _mapper.Map<AnswerDto>(answer);
            dto.Rating = answer.Rating; // Sử dụng property computed Rating từ entity
            dto.VoteCount = answer.VoteCount; // Mới: Map VoteCount
            return dto;
        }

        public async Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto dto)
        {
            var answer = _mapper.Map<Answer>(dto);
            answer.CreatedAt = DateTime.UtcNow;
            var created = await _repository.CreateAnswerAsync(answer);

            // Fix: Lấy full entity với populate (CreatedBy, Rating, etc.)
            var fullAnswer = await GetAnswerByIdAsync(created.AnswerId);
            if (fullAnswer == null)
            {
                throw new Exception("Error retrieving created answer");
            }

            return fullAnswer;  // Trả full DTO với CreatedBy đầy đủ
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

        // CRUD for Vote (thêm methods cho Vote)
        public async Task<IEnumerable<VoteDto>> GetVotesByAnswerIdAsync(int answerId)
        {
            var votes = await _repository.GetVotesByAnswerIdAsync(answerId);
            return _mapper.Map<IEnumerable<VoteDto>>(votes);
        }

        public async Task<VoteDto?> GetVoteByIdAsync(int id)
        {
            var vote = await _repository.GetVoteByIdAsync(id);
            return _mapper.Map<VoteDto>(vote);
        }

        public async Task<VoteDto?> GetVoteByAnswerAndUserAsync(int answerId, int userId)
        {
            var vote = await _repository.GetVoteByAnswerAndUserAsync(answerId, userId);
            return _mapper.Map<VoteDto>(vote);
        }

        public async Task<VoteDto> CreateVoteAsync(CreateVoteDto dto)
        {
            var vote = _mapper.Map<Vote>(dto);
            vote.CreatedAt = DateTime.UtcNow;
            var created = await _repository.CreateVoteAsync(vote);
            return _mapper.Map<VoteDto>(created);
        }

        public async Task UpdateVoteAsync(int id, UpdateVoteDto dto)
        {
            var vote = await _repository.GetVoteByIdAsync(id);
            if (vote != null)
            {
                _mapper.Map(dto, vote);
                vote.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateVoteAsync(vote);
            }
        }

        public async Task DeleteVoteAsync(int id)
        {
            await _repository.DeleteVoteAsync(id);
        }
    }
}