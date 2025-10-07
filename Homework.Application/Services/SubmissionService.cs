using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Homework.Domain.DTOs;
using Homework.Domain.Entities;
using Homework.Domain.Repositories;

namespace Homework.Application.Services
{
    public class SubmissionService
    {
        private readonly ISubmissionRepository _repo;
        private readonly IMapper _mapper;

        public SubmissionService(ISubmissionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubmissionReadDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<SubmissionReadDto>>(list);
        }

        public async Task<IEnumerable<SubmissionReadDto>> GetByHomeworkIdAsync(int homeworkId)
        {
            var list = await _repo.GetByHomeworkIdAsync(homeworkId);
            return _mapper.Map<IEnumerable<SubmissionReadDto>>(list);
        }

        public async Task<SubmissionReadDto?> GetByIdAsync(int id)
        {
            var sub = await _repo.GetByIdAsync(id);
            return sub == null ? null : _mapper.Map<SubmissionReadDto>(sub);
        }

        public async Task<SubmissionReadDto> CreateAsync(SubmissionCreateDto dto)
        {
            var entity = _mapper.Map<Submission>(dto);
            entity.SubmittedAt = DateTime.UtcNow;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return _mapper.Map<SubmissionReadDto>(entity);
        }
        public async Task<SubmissionReadDto?> GetByHomeworkAndUserAsync(int homeworkId, int userId)
        {
            var list = await _repo.GetByHomeworkIdAsync(homeworkId);
            var sub = list.FirstOrDefault(s => s.UserId == userId);
            return sub == null ? null : _mapper.Map<SubmissionReadDto>(sub);
        }

        public async Task<bool> UpdateAsync(int id, SubmissionUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return false;

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
