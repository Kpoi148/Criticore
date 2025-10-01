using Homework.Application.Services;
using Homework.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeWorksController : ControllerBase
    {
        // Thay đổi kiểu dữ liệu từ interface sang class
        private readonly HomeworkService _service;

        // Thay đổi kiểu dữ liệu tham số constructor
        public HomeWorksController(HomeworkService service)
        {
            _service = service;
        }

        // POST: api/homeworks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HomeworkCreateDto dto)
        {
            // TODO: Lấy userId từ HttpContext/Claims/Token
            int createdByUserId = 1; // Giả định UserId

            var created = await _service.AddAsync(dto, createdByUserId);

            return CreatedAtAction(nameof(GetById), new { id = created.HomeworkID }, created);
        }

        // GET: api/homeworks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var homework = await _service.GetByIdAsync(id);
            return homework == null ? NotFound() : Ok(homework);
        }

        // GET: api/topics/{topicId}/homeworks
        [HttpGet("/api/topics/{topicId}/homeworks")]
        public async Task<IActionResult> GetByTopic(int topicId)
        {
            var homeworks = await _service.GetByTopicAsync(topicId);
            return Ok(homeworks);
        }

        // PUT: api/homeworks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HomeworkDto dto)
        {
            if (id != dto.HomeworkID)
            {
                return BadRequest("HomeworkID trong URL không khớp với dữ liệu.");
            }

            await _service.UpdateAsync(dto);
            return NoContent(); // 204 No Content
        }

        // DELETE: api/homeworks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent(); // 204 No Content
        }
    }
}

