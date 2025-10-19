using Homework.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homework.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CopyleaksReportController : ControllerBase
    {
        private readonly CopyleaksReportService _reportService;

        public CopyleaksReportController(CopyleaksReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/CopyleaksReport/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReportsByUser(int userId)
        {
            var reports = await _reportService.GetReportsByUserAsync(userId);

            if (reports == null)
                return NotFound(new { success = false, message = "No reports found for this user." });

            return Ok(new { success = true, data = reports });
        }
    }
}
