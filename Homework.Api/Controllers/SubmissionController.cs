using Homework.Application.Services;
using Homework.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly SubmissionService _service;
        public SubmissionController(SubmissionService service)
        {
            _service = service;
        }
        // Lấy tất cả submissions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _service.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sub = await _service.GetByIdAsync(id);
            if (sub == null)
                return NotFound();
            return Ok(sub);
        }

        // Lấy submissions theo homeworkId
        [HttpGet("homework/{homeworkId}")]
        public async Task<IActionResult> GetByHomework(int homeworkId)
        {
            var filtered = await _service.GetByHomeworkIdAsync(homeworkId);
            return Ok(filtered);
        }

        // Lấy submission của 1 học sinh theo homeworkId
        [HttpGet("homework/{homeworkId}/user/{userId}")]
        public async Task<IActionResult> GetByHomeworkAndUser(int homeworkId, int userId)
        {
            var submission = await _service.GetByHomeworkAndUserAsync(homeworkId, userId);
            if (submission == null)
                return NotFound();
            return Ok(submission);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubmissionCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.SubmissionId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubmissionUpdateDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound();
            var updated = await _service.GetByIdAsync(id);
            return Ok(updated); // trả 200 với SubmissionReadDto
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
