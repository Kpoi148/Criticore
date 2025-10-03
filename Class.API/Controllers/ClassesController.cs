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

        // Lấy danh sách lớp
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _service.GetAllAsync();
            return Ok(classes);
        }

        // Lấy chi tiết lớp theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cls = await _service.GetByIdAsync(id);
            return cls == null ? NotFound() : Ok(cls);
        }

        // Tạo mới lớp
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassCreateDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ClassId }, created);
        }
        // Cập nhật lớp
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassDto dto)
        {
            if (id != dto.ClassId) return BadRequest("ClassId không khớp.");
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        // Xóa lớp
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
        // Thêm mới: Xem danh sách thành viên lớp
        [HttpGet("{id}/teachers")]
        public async Task<IActionResult> GetTeachers(int id)
        {
            var teachers = await _service.GetTeachersByClassAsync(id);
            return Ok(teachers);
        }

        // Thêm mới: Xóa thành viên khỏi lớp
        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveMemberInClass(int id, int memberId)
        {
            // Kiểm tra classId khớp (tùy chọn, để bảo mật)
            await _service.RemoveMemberFromClassAsync(memberId);
            return NoContent();
        }

        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMember(int classId, int groupId, [FromBody] int classMemberId)
        {
            // Kiểm tra classId hợp lệ nếu cần
            await _groupservice.AddMemberToGroupAsync(groupId, classMemberId);
            return Ok(new { message = "Member added to group successfully" });
        }

        [HttpDelete("{id}/groups/{groupId}/members/{classMemberId}")]
        public async Task<IActionResult> RemoveMemberFromGroup(int id, int groupId, int classMemberId)
        {
            // Kiểm tra id (classId) hợp lệ nếu cần (trong service)
            await _groupservice.RemoveMemberFromGroupAsync(groupId, classMemberId);
            return NoContent();
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetClassesByUser(int userId)
        {
            var classes = await _service.GetClassesByUserAsync(userId);
            return Ok(classes);
        }
        // Gán giáo viên cho lớp
        [HttpPut("{id}/assign-teacher/{teacherId}")]
        public async Task<IActionResult> AssignTeacher(int id, int teacherId)
        {
            var success = await _service.AssignTeacherAsync(id, teacherId);
            if (!success) return BadRequest("Không thể gán giáo viên.");
            return NoContent();
        }

    }
}
