using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class.Domain.DTOs;
using Class.Domain.Entities;
using Class.Domain.Repositories;

namespace Class.Application.Services
{

    public class JoinRequestService
    {
        private readonly IJoinRequestRepository _repo;

        public JoinRequestService(IJoinRequestRepository repo)
        {
            _repo = repo;
        }

        public Task AddAsync(JoinRequest joinRequest) => _repo.AddAsync(joinRequest);
        public Task<IEnumerable<JoinRequest>> GetByClassIdAsync(int classId) => _repo.GetByClassIdAsync(classId);
        public Task<JoinRequest?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task<JoinRequest?> ReviewAsync(ReviewJoinRequestDto dto)
        {
            var joinRequest = await _repo.GetByIdAsync(dto.JoinRequestId);
            if (joinRequest == null) return null;

            joinRequest.Status = dto.Status;
            joinRequest.ReviewedBy = dto.ReviewedBy;
            joinRequest.ReviewedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(joinRequest);
            return joinRequest;
        }
    }
}
