using Class.Application.Services;
using Class.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Class.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly ClassService _service;
        public ClassesController(ClassService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cls = await _service.GetByIdAsync(id);
            return cls == null ? NotFound() : Ok(cls);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Class.Domain.Entities.Class cls)
        {
            await _service.AddAsync(cls);
            return CreatedAtAction(nameof(GetById), new { id = cls.ClassId }, cls);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Class.Domain.Entities.Class cls)
        {
            if (id != cls.ClassId) return BadRequest();
            await _service.UpdateAsync(cls);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        // Tham gia lớp bằng mã mới, khi nhập mã sẽ tìm theo đúng lớp với mã đó và thêm học sinh vào lớp
        [HttpPost("join-by-code")]
        public async Task<IActionResult> JoinByCode([FromBody] JoinByCodeDto dto)
        {
            var cls = await _service.JoinByCodeAsync(dto.JoinCode, dto.UserId);
            if (cls == null) return NotFound("Invalid join code");

            return Ok(new { message = "Joined class successfully", classId = cls.ClassId });
        }
    }
}
