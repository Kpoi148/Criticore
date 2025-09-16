using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.Repositories;

namespace Class.Application.Services
{
    public class ClassService
    {
        private readonly IClassRepository _classRepository;

        public ClassService(IClassRepository classRepository) { _classRepository = classRepository; }

        public Task<IEnumerable<Class.Domain.Entities.Class>> GetAllAsync() => _classRepository.GetAllAsync();
        public Task<Class.Domain.Entities.Class?> GetByIdAsync(int id) => _classRepository.GetByIdAsync(id);
        public Task AddAsync(Class.Domain.Entities.Class cls) => _classRepository.AddAsync(cls);
        public Task UpdateAsync(Class.Domain.Entities.Class cls) => _classRepository.UpdateAsync(cls);
        public Task DeleteAsync(int id) => _classRepository.DeleteAsync(id);
    }
}

