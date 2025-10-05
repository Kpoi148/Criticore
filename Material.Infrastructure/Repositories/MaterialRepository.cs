using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Material.Domain.Repositories;
using Material.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Material.Infrastructure.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly MaterialContext _context;

        public MaterialRepository(MaterialContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Material> AddAsync(Domain.Entities.Material material)
        {
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        public async Task<Domain.Entities.Material?> GetByIdAsync(int id)
        {
            return await _context.Materials
                                 .FirstOrDefaultAsync(m => m.MaterialId == id);
        }

        public async Task<IEnumerable<Domain.Entities.Material>> GetByClassIdAsync(int classId)
        {
            return await _context.Materials
                                 .Where(m => m.ClassId == classId)
                                 .OrderByDescending(m => m.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Material>> GetAllAsync()
        {
            return await _context.Materials
                                 .OrderByDescending(m => m.CreatedAt)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Domain.Entities.Material>> GetByHomeworkIdAsync(int homeworkId)
        {
            return await _context.Materials
                                 .Where(m => m.HomeworkId == homeworkId)
                                 .OrderByDescending(m => m.CreatedAt)
                                 .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
            }
        }
    }
}
