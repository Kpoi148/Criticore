using AutoMapper;
using Class.Domain.DTOs;
using Class.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Application.Services
{
    public class ClassService
    {
        private readonly IClassRepository _classRepo;
        private readonly IClassMemberRepository _memberRepo;
        private readonly IMapper _mapper;

        public ClassService(IClassRepository classRepo, IClassMemberRepository memberRepo, IMapper mapper)
        {
            _classRepo = classRepo;
            _memberRepo = memberRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassDto>> GetAllAsync()
        {
            var entities = await _classRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ClassDto>>(entities);
        }

        public async Task<ClassDto?> GetByIdAsync(int id)
        {
            var entity = await _classRepo.GetByIdAsync(id);
            return _mapper.Map<ClassDto?>(entity);
        }

        public async Task<ClassDto> AddAsync(ClassDto dto)
        {
            var entity = _mapper.Map<Class.Domain.Entities.Class>(dto);
            await _classRepo.AddAsync(entity);
            return _mapper.Map<ClassDto>(entity); // entity lúc này có ClassId
        }

        public async Task UpdateAsync(ClassDto dto)
        {
            var entity = _mapper.Map<Class.Domain.Entities.Class>(dto);
            await _classRepo.UpdateAsync(entity);
        }

        public Task DeleteAsync(int id) => _classRepo.DeleteAsync(id);

        public async Task<ClassDto?> JoinByCodeAsync(string joinCode, int userId)
        {
            var cls = await _classRepo.GetByJoinCodeAsync(joinCode);
            if (cls == null) return null;

            await _memberRepo.AddStudentToClassAsync(cls.ClassId, userId);
            return _mapper.Map<ClassDto>(cls);
        }

        public async Task<List<ClassMemberDto>> GetMembersByClassAsync(int classId)
        {
            var members = await _memberRepo.GetMembersByClassAsync(classId);
            return _mapper.Map<List<ClassMemberDto>>(members);
        }

        public Task RemoveMemberFromClassAsync(int classMemberId)
            => _memberRepo.RemoveMemberFromClassAsync(classMemberId);
    }
}


