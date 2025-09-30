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
    public class ClassRepository : IClassRepository
    {
        private readonly ClassDbContext _context;

        public ClassRepository(ClassDbContext context) { _context = context; }
        public async Task<IEnumerable<Class.Domain.Entities.Class>> GetAllAsync()
          => await _context.Classes.ToListAsync();

        public async Task<Class.Domain.Entities.Class?> GetByIdAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.ClassMembers) // nạp thêm Members
                    .ThenInclude(m => m.User) // load thông tin user
                .FirstOrDefaultAsync(c => c.ClassId == id);
        }

        public async Task AddAsync(Class.Domain.Entities.Class cls)
        {
            _context.Classes.Add(cls);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Class.Domain.Entities.Class cls)
        {
            _context.Classes.Update(cls);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls != null)
            {
                _context.Classes.Remove(cls);
                await _context.SaveChangesAsync();
            }

        }
        public async Task<Class.Domain.Entities.Class?> GetByJoinCodeAsync(string joinCode)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.JoinCode == joinCode);
        }
    }
}
