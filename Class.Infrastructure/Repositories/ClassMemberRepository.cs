using Class.Domain.DTOs;
using Class.Domain.Entities;
using Class.Domain.Repositories;
using Class.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Infrastructure.Repositories
{
    public class ClassMemberRepository : IClassMemberRepository
    {
        private readonly ClassDbContext _db;
        private readonly IGroupRepository _groupRepo;
        public ClassMemberRepository(ClassDbContext db, IGroupRepository groupRepo)
        {
            _db = db;
            _groupRepo = groupRepo;
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

        // Thêm mới: Xem thành viên lớp
        public async Task<List<ClassMemberDto>> GetMembersByClassAsync(int classId)
        {
            var members = await _db.ClassMembers
                .Where(cm => cm.ClassId == classId)
                .Include(cm => cm.User) // Include User để lấy info
                .ToListAsync();

            return members.Select(cm => new ClassMemberDto
            {
                ClassMemberId = cm.ClassMemberId,
                UserId = cm.UserId,
                FullName = cm.User.FullName ?? string.Empty, // Giả sử User có Name
                Email = cm.User.Email ?? string.Empty, // Giả sử User có Email
                RoleInClass = cm.RoleInClass ?? string.Empty,
                JoinedAt = cm.JoinedAt,
                GroupId = cm.GroupId
            }).ToList();
        }

        // Thêm mới: Xóa thành viên
        public async Task RemoveMemberFromClassAsync(int classMemberId)
        {
            var member = await _db.ClassMembers
                .Include(m => m.Group) // Nếu cần, tùy cấu trúc entity
                .FirstOrDefaultAsync(m => m.ClassMemberId == classMemberId);

            if (member == null)
            {
                throw new ArgumentException($"Member {classMemberId} not found");
            }

            // Nếu đang trong group, xóa khỏi group trước
            if (member.GroupId != null)
            {
                await _groupRepo.RemoveMemberFromGroupAsync(member.GroupId.Value, classMemberId);
            }

            _db.ClassMembers.Remove(member);
            await _db.SaveChangesAsync();
        }
    }
}
