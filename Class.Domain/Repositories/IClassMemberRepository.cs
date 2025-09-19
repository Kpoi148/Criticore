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

        // Thêm mới: Xem danh sách thành viên lớp
        Task<List<ClassMemberDto>> GetMembersByClassAsync(int classId);

        // Thêm mới: Xóa thành viên khỏi lớp (sử dụng ClassMemberId để chính xác)
        Task RemoveMemberFromClassAsync(int classMemberId);
    }
}
