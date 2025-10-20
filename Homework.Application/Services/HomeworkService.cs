using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Homework.Domain.DTOs;
using Homework.Domain.Repositories;

namespace Homework.Application.Services
{
    public class HomeworkService
    {
        private readonly IHomeworkRepository _homeworkRepo;
        private readonly IMapper _mapper;

        public HomeworkService(IHomeworkRepository homeworkRepo, IMapper mapper)
        {
            _homeworkRepo = homeworkRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<HomeworkDto>> GetAllAsync()
        {
            var entities = await _homeworkRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<HomeworkDto>>(entities);
        }

        public async Task<HomeworkDto?> GetByIdAsync(int id)
        {
            var entity = await _homeworkRepo.GetByIdAsync(id);
            return _mapper.Map<HomeworkDto?>(entity);
        }

        public async Task<IEnumerable<HomeworkDto>> GetByTopicAsync(int topicId)
        {
            var entities = await _homeworkRepo.GetByTopicAsync(topicId);
            return _mapper.Map<IEnumerable<HomeworkDto>>(entities);
        }

        public async Task<HomeworkDto> AddAsync(HomeworkCreateDto dto, int createdByUserId)
        {
            var entity = _mapper.Map<Homework.Domain.Entities.HomeWork>(dto);

            // Gán các giá trị từ server/context
            entity.Status = "Open";
            entity.CreatedBy = createdByUserId;
            entity.CreatedAt = DateTime.Now;

            await _homeworkRepo.AddAsync(entity);
            return _mapper.Map<HomeworkDto>(entity); // entity lúc này đã có HomeworkID
        }

        public async Task UpdateAsync(HomeworkDto dto)
        {
            var existingEntity = await _homeworkRepo.GetByIdAsync(dto.HomeworkID);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Homework with ID {dto.HomeworkID} not found.");
            }

            // Ánh xạ các thuộc tính từ DTO sang Entity hiện có
            _mapper.Map(dto, existingEntity);

            // Cập nhật trường UpdatedAt
            existingEntity.UpdatedAt = DateTime.Now;

            await _homeworkRepo.UpdateAsync(existingEntity);
        }

        public async Task<List<DeadlineDto>> GetDeadlinesByUserAsync(int userId)
        {
            var deadlines = await _homeworkRepo.GetDeadlinesByUserAsync(userId);
            // Nếu cần map thêm (ví dụ: từ entity sang DTO phức tạp), dùng _mapper.Map
            // Nhưng ở đây repo đã return DTO trực tiếp, nên return thẳng
            return deadlines;
        }

        public Task DeleteAsync(int id) => _homeworkRepo.DeleteAsync(id);
    }
}
