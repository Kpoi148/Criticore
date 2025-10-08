using Homework.Domain.DTOs;
using Homework.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Homework.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CopyleaksWebhookController : ControllerBase
    {
        private readonly HomeworkDbContext _context;
        public CopyleaksWebhookController(HomeworkDbContext context)
        {
            _context = context;
        }

        [HttpPost("webhook/completed")]
        public async Task<IActionResult> OnScanCompleted([FromBody] dynamic data)
        {
            // Lấy scanId để tìm submission
            string scanId = data.scanId;

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.SubmissionId.ToString() == scanId); // hoặc mapping scanId → SubmissionID

            if (submission == null)
                return NotFound($"Submission for scanId={scanId} not found");

            // Lấy dữ liệu Copyleaks
            double plagiarismScore = (double)(data.results?.internet?.score ?? 0)
                                   + (double)(data.results?.database?.score ?? 0);
            double aiContentScore = (double)(data.results?.ai?.score ?? 0);
            string reportUrl = data.pdfReport ?? "";

            //// Update vào submission
            //submission.PlagiarismScore = plagiarismScore * 100;
            //submission.AiContentScore = aiContentScore * 100;
            //submission.ReportURL = reportUrl;
            //submission.Status = "Completed";
            submission.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Submission updated"
                //, submissionId = submission.SubmissionID 
            });
        }

        [HttpPost("webhook/error")]
        public IActionResult OnScanError([FromBody] dynamic error)
        {
            return BadRequest(error);
        }
    }
}
