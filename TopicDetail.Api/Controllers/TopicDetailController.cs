using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TopicDetail.Application.DTOs;
using TopicDetail.Application.Services;

namespace TopicDetail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicDetailController : ControllerBase
    {
        private readonly TopicDetailService _service;

        public TopicDetailController(TopicDetailService service)
        {
            _service = service;
        }

        // CRUD for Answer
        [HttpGet("topics/{topicId}/answers")]
        public async Task<ActionResult<IEnumerable<AnswerDto>>> GetAnswersByTopicId(int topicId)
        {
            return Ok(await _service.GetAnswersByTopicIdAsync(topicId));
        }

        [HttpGet("answers/{id}")]
        public async Task<ActionResult<AnswerDto>> GetAnswerById(int id)
        {
            var answer = await _service.GetAnswerByIdAsync(id);
            return answer != null ? Ok(answer) : NotFound();
        }

        [HttpPost("answers")]
        public async Task<ActionResult<AnswerDto>> CreateAnswer([FromBody] CreateAnswerDto dto)
        {
            var created = await _service.CreateAnswerAsync(dto);
            return CreatedAtAction(nameof(GetAnswerById), new { id = created.AnswerId }, created);
        }

        [HttpPut("answers/{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] UpdateAnswerDto dto)
        {
            await _service.UpdateAnswerAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("answers/{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            await _service.DeleteAnswerAsync(id);
            return NoContent();
        }
    }
}