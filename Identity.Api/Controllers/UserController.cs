using Identity.Application.Services;
using Identity.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            var teachers = await _userService.GetTeachersAsync();
            return Ok(teachers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var created = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (id != dto.UserId) return BadRequest("Id mismatch");

            var updated = await _userService.UpdateAsync(dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        [HttpPut("{id:int}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromQuery] int roleType)
        {
            string role = roleType switch
            {
                1 => "User",
                2 => "Teacher",
                _ => throw new ArgumentException("Role type must be 1 (User) or 2 (Teacher)")
            };

            var updated = await _userService.ChangeRoleAsync(id, role);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpPut("{id:int}/ban")]
        public async Task<IActionResult> Ban(int id)
        {
            var bannedUser = await _userService.BanAsync(id);
            if (bannedUser == null) return NotFound();

            return Ok(bannedUser);
        }
        [HttpPut("{id:int}/unban")]
        public async Task<IActionResult> Unban(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userService.UnbanAsync(id);
            return Ok(result);
        }
    }
}