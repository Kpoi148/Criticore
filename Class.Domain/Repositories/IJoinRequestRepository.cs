using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.Entities;

namespace Class.Domain.Repositories
{
    public interface IJoinRequestRepository
    {
        Task AddAsync(JoinRequest joinRequest);
        Task<IEnumerable<JoinRequest>> GetByClassIdAsync(int classId);
        Task<JoinRequest?> GetByIdAsync(int id);
        Task UpdateAsync(JoinRequest joinRequest); // để review
    }
}
