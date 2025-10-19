using Homework.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homework.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CopyleaksController : ControllerBase
    {
        private readonly CopyleaksService _copyleaksService;

        public CopyleaksController(CopyleaksService service)
        {
            _copyleaksService = service;
        }

        [HttpPost("check-file")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CheckFile([FromForm] CheckFileRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var scanId = await _copyleaksService.SubmitFileForScanAsync(request.File, request.UserId);
            return Ok(new { message = "File submitted successfully.", scanId });
        }
    }

    public class CheckFileRequest
    {
        public IFormFile File { get; set; } = default!;
        public int UserId { get; set; }
    }
}
