using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.Repositories;
using Class.Infrastructure.Models;
using Class.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Class.Infrastructure.Repositories
{
    public class ClassMemberRepository : IClassMemberRepository
    {
        private readonly ClassDbContext _db;

        public ClassMemberRepository(ClassDbContext db)
        {
            _db = db;
        }

        public async Task<ClassMember?> GetByClassAndUserAsync(int classId, int userId)
        {
            return await _db.ClassMembers
                .FirstOrDefaultAsync(cm => cm.ClassId == classId && cm.UserId == userId);
        }

        public async Task<ClassMember> AddStudentToClassAsync(int classId, int userId)
        {
            var existed = await GetByClassAndUserAsync(classId, userId);
            if (existed != null) return existed;

            var member = new ClassMember
            {
                ClassId = classId,
                UserId = userId,
                RoleInClass = "Student",
                JoinedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            _db.ClassMembers.Add(member);
            await _db.SaveChangesAsync();
            return member;
        }
    }
}
