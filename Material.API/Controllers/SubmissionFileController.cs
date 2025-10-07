using Material.Application.Services;
using Material.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Material.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionFileController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;

        public SubmissionFileController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] SubmissionFileUploadDto dto)
        {
            var fileUrl = await _cloudinaryService.UploadFileAsync(dto.File, "submission_files");
            return Ok(new { message = "Upload thành công", fileUrl });
        }
    }
}
