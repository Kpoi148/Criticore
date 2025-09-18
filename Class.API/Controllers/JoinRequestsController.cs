using Class.Application.Services;
using Class.Domain.DTOs;
using Class.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Class.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JoinRequestsController : ControllerBase
    {
        private readonly JoinRequestService _service;

        public JoinRequestsController(JoinRequestService service)
        {
            _service = service;
        }

        // POST api/joinrequests
        [HttpPost]
        public async Task<IActionResult> CreateJoinRequest([FromBody] CreateJoinRequestDto dto)
        {
            var joinRequest = new JoinRequest
            {
                ClassId = dto.ClassId,
                UserId = dto.UserId,
                Message = dto.Message
            };

            await _service.AddAsync(joinRequest);
            return Ok(joinRequest);
        }

        // GET api/joinrequests/class/5
        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetByClass(int classId)
        {
            var requests = await _service.GetByClassIdAsync(classId);

            var result = requests.Select(r => new JoinRequestDto
            {
                JoinRequestId = r.JoinRequestId,
                ClassId = r.ClassId,
                UserId = r.UserId,
                Message = r.Message,
                FullName = r.User?.FullName,
                Email = r.User?.Email ?? string.Empty
            });
            return Ok(result);
        }
        //PUT /api/joinRequests/review
        [HttpPut("review")]
        public async Task<IActionResult> Review([FromBody] ReviewJoinRequestDto dto)
        {
            var updated = await _service.ReviewAsync(dto);
            if (updated == null) return NotFound();

            var result = new JoinRequestDto
            {
                JoinRequestId = updated.JoinRequestId,
                ClassId = updated.ClassId,
                UserId = updated.UserId,
                Message = updated.Message,
                Status = updated.Status ?? "Pending",
                FullName = updated.User?.FullName,
                Email = updated.User?.Email ?? ""
            };
            return Ok(result);
        }
    }
}
