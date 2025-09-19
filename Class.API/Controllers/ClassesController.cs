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
        private readonly GroupService _groupservice;
        public ClassesController(ClassService service, GroupService groupService)
        {
            _service = service;
            _groupservice = groupService;
        }

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

        // Thêm mới: Xem danh sách thành viên lớp
        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMembers(int id)
        {
            var members = await _service.GetMembersByClassAsync(id);
            return Ok(members);
        }

        // Thêm mới: Xóa thành viên khỏi lớp
        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveMemberInClass(int id, int memberId)
        {
            // Kiểm tra classId khớp (tùy chọn, để bảo mật)
            await _service.RemoveMemberFromClassAsync(memberId);
            return NoContent();
        }

        // Các endpoint khác cho Group (create, get, update, delete) có thể thêm ở đây

        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMember(int classId, int groupId, [FromBody] int classMemberId)
        {
            // Kiểm tra classId hợp lệ nếu cần
            await _groupservice.AddMemberToGroupAsync(groupId, classMemberId);
            return Ok();
        }

        [HttpDelete("{id}/groups/{groupId}/members/{classMemberId}")]
        public async Task<IActionResult> RemoveMemberFromGroup(int id, int groupId, int classMemberId)
        {
            // Kiểm tra id (classId) hợp lệ nếu cần (trong service)
            await _groupservice.RemoveMemberFromGroupAsync(groupId, classMemberId);
            return NoContent();
        }
    }
}
