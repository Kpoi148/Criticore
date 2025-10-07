using ChatBot.Application.Services.Interfaces;
using ChatBot.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly IAiInteractionService _service;

        public AiController(IAiInteractionService service) { _service = service; }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskAiRequest request)
        {
            // Lấy user/class từ HttpContext hoặc session (giả sử từ Identity)
            string userId = HttpContext.User.FindFirst("sub")?.Value ?? "unknown";  // Từ JWT nếu auth
            string userName = HttpContext.User.Identity?.Name ?? "Unknown";
            string classId = HttpContext.Session.GetString("currentClassId") ?? "default";  // Hoặc từ request

            try
            {
                var result = await _service.AskAiAsync(request, userId, userName, classId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to process AI request: " + ex.Message });
            }
        }
    }
}
