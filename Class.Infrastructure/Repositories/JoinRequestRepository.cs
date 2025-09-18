using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.Entities;
using Class.Domain.Repositories;
using Class.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Class.Infrastructure.Repositories
{
    public class JoinRequestRepository : IJoinRequestRepository
    {
        private readonly ClassDbContext _context;
        private readonly IClassMemberRepository _classMemberRepo;

        public JoinRequestRepository(ClassDbContext context, IClassMemberRepository classMemberRepo)
        {
            _context = context;
            _classMemberRepo = classMemberRepo;
        }

        public async Task AddAsync(JoinRequest joinRequest)
        {
            _context.JoinRequests.Add(joinRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<JoinRequest>> GetByClassIdAsync(int classId)
        {
            return await _context.JoinRequests
                .Include(j => j.User)
                .Where(j => j.ClassId == classId)
                .ToListAsync();
        }

        public async Task<JoinRequest?> GetByIdAsync(int id)
        {
            return await _context.JoinRequests
                .Include(j => j.User)
                .Include(j => j.Class)
                .FirstOrDefaultAsync(j => j.JoinRequestId == id);
        }

        public async Task UpdateAsync(JoinRequest joinRequest)
        {
            _context.JoinRequests.Update(joinRequest);
            await _context.SaveChangesAsync();
        }

    }
}
