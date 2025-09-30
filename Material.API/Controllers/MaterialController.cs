using Material.Application.Services;
using Material.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Material.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly MaterialService _service;

        public MaterialController(MaterialService service)
        {
            _service = service;
        }

        // Upload tài liệu
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] MaterialUploadDto dto)
        {
            await _service.UploadAndSaveAsync(dto.File, dto.ClassId, dto.UploadedBy);
            return Ok(new { message = "Tải lên tài liệu thành công" });
        }

        // Lấy danh sách tài liệu theo class
        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetByClass(int classId)
        {
            var list = await _service.GetByClassIdAsync(classId);
            return Ok(list);
        }

        // Lấy tài liệu theo id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var material = await _service.GetByIdAsync(id);
            if (material == null) return NotFound();
            return Ok(material);
        }

        // Xóa tài liệu
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
