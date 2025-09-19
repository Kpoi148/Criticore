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

        public ClassService(IClassRepository classRepo, IClassMemberRepository memberRepo)
        {
            _classRepo = classRepo;
            _memberRepo = memberRepo;
        }
        public Task<IEnumerable<Class.Domain.Entities.Class>> GetAllAsync() => _classRepo.GetAllAsync();
        public Task<Class.Domain.Entities.Class?> GetByIdAsync(int id) => _classRepo.GetByIdAsync(id);
        public Task AddAsync(Class.Domain.Entities.Class cls) => _classRepo.AddAsync(cls);
        public Task UpdateAsync(Class.Domain.Entities.Class cls) => _classRepo.UpdateAsync(cls);
        public Task DeleteAsync(int id) => _classRepo.DeleteAsync(id);
        // Tham gia lớp bằng mã mới, khi nhập mã sẽ tìm theo đúng lớp với mã đó và thêm học sinh vào lớp
        public async Task<Class.Domain.Entities.Class?> JoinByCodeAsync(string joinCode, int userId)
        {
            // Tim lớp theo mã học sinh nhập
            var cls = await _classRepo.GetByJoinCodeAsync(joinCode);
            if (cls == null) return null;

            await _memberRepo.AddStudentToClassAsync(cls.ClassId, userId);
            return cls;
        }
        // Thêm mới: Xem danh sách thành viên lớp
        public Task<List<ClassMemberDto>> GetMembersByClassAsync(int classId)
            => _memberRepo.GetMembersByClassAsync(classId);

        // Thêm mới: Xóa thành viên khỏi lớp
        public Task RemoveMemberFromClassAsync(int classMemberId)
            => _memberRepo.RemoveMemberFromClassAsync(classMemberId);
    }
}

