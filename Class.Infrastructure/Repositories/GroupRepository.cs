using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.Repositories;
using Class.Infrastructure.Models;
using Microsoft.EntityFrameworkCore; // Giả sử sử dụng EF Core
namespace Class.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ClassDbContext _context; // Giả sử DbContext của ứng dụng

        public GroupRepository(ClassDbContext context)
        {
            _context = context;
        }

        public async Task AddMemberToGroupAsync(int groupId, int classMemberId)
        {
            var group = await _context.Groups.Include(g => g.ClassMembers).FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null) throw new Exception("Group not found");

            var member = await _context.ClassMembers.FirstOrDefaultAsync(m => m.ClassMemberId == classMemberId); // Giả sử ClassMember có ClassMemberId
            if (member == null) throw new Exception("Member not found");

            if (member.GroupId != null) throw new Exception("Member already in a group");

            member.GroupId = groupId;
            group.ClassMembers.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberFromGroupAsync(int groupId, int classMemberId)
        {
            var group = await _context.Groups.Include(g => g.ClassMembers).FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null) throw new Exception("Group not found");

            var member = group.ClassMembers.FirstOrDefault(m => m.ClassMemberId == classMemberId);
            if (member == null) throw new Exception("Member not in group");

            group.ClassMembers.Remove(member);
            member.GroupId = null;
            await _context.SaveChangesAsync();
        }
    }
}