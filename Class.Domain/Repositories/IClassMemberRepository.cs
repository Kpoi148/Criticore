using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.Entities;

namespace Class.Domain.Repositories
{
    public interface IClassMemberRepository
    {
        Task<ClassMember?> GetByClassAndUserAsync(int classId, int userId);
        Task<ClassMember> AddStudentToClassAsync(int classId, int userId);
    }
}
