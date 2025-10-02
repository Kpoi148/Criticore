using Class.Domain.DTOs;
using Class.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Domain.Repositories
{
    public interface IClassMemberRepository
    {
        Task<ClassMember?> GetByClassAndUserAsync(int classId, int userId);
        Task<ClassMember> AddStudentToClassAsync(int classId, int userId);
        // Thêm thành viên vào lớp
        Task AddAsync(ClassMember member);

        // Xem danh sách thành viên lớp
        Task<List<ClassMemberDto>> GetMembersByClassAsync(int classId);

        // Xóa thành viên khỏi lớp (sử dụng ClassMemberId để chính xác)
        Task RemoveMemberFromClassAsync(int classMemberId);
        // Lấy danh sách lớp theo userId
        Task<List<Class.Domain.Entities.Class>> GetClassesByUserAsync(int userId);

    }
}
