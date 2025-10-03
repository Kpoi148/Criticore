using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using TopicDetail.Api.Hubs;
using TopicDetail.Application.DTOs;
using TopicDetail.Application.Services;
namespace TopicDetail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicDetailController : ControllerBase
    {
        private readonly TopicDetailService _service;
        private readonly IHubContext<TopicHub> _hubContext;  // Inject HubContext

        public TopicDetailController(TopicDetailService service, IHubContext<TopicHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
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

            // Push real-time: Gửi event "NewAnswer" đến tất cả clients (hoặc group nếu dùng)
            await _hubContext.Clients.All.SendAsync("NewAnswer", created);  // Gửi object AnswerDto

            return CreatedAtAction(nameof(GetAnswerById), new { id = created.AnswerId }, created);
        }

        [HttpPut("answers/{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] UpdateAnswerDto dto)
        {
            await _service.UpdateAnswerAsync(id, dto);

            // Push real-time: Gửi event "UpdatedAnswer"
            await _hubContext.Clients.All.SendAsync("UpdatedAnswer", new { AnswerId = id, UpdatedContent = dto.Content });  // Gửi data cần thiết

            return NoContent();
        }

        [HttpDelete("answers/{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            await _service.DeleteAnswerAsync(id);

            // Push real-time: Gửi event "DeletedAnswer"
            await _hubContext.Clients.All.SendAsync("DeletedAnswer", id);

            return NoContent();
        }
        // CRUD for Vote (thêm các endpoint cho Vote)
        [HttpGet("answers/{answerId}/votes")]
        public async Task<ActionResult<IEnumerable<VoteDto>>> GetVotesByAnswerId(int answerId)
        {
            return Ok(await _service.GetVotesByAnswerIdAsync(answerId));
        }
        [HttpGet("votes/{id}")]
        public async Task<ActionResult<VoteDto>> GetVoteById(int id)
        {
            var vote = await _service.GetVoteByIdAsync(id);
            return vote != null ? Ok(vote) : NotFound();
        }
        [HttpPost("votes")]
        public async Task<ActionResult<VoteDto>> CreateOrUpdateVote([FromBody] CreateVoteDto dto)
        {
            // Kiểm tra nếu user đã vote cho answer này chưa
            var existingVote = await _service.GetVoteByAnswerAndUserAsync(dto.AnswerId, dto.UserId);
            if (existingVote != null)
            {
                // Update vote existing
                var updateDto = new UpdateVoteDto
                {
                    VoteType = dto.VoteType,
                    Amount = dto.Amount
                };
                await _service.UpdateVoteAsync(existingVote.VoteId, updateDto);
                return Ok(await _service.GetVoteByIdAsync(existingVote.VoteId));
            }
            else
            {
                // Create new vote
                var created = await _service.CreateVoteAsync(dto);
                return CreatedAtAction(nameof(GetVoteById), new { id = created.VoteId }, created);
            }

        }
        [HttpPut("votes/{id}")]
        public async Task<IActionResult> UpdateVote(int id, [FromBody] UpdateVoteDto dto)
        {
            await _service.UpdateVoteAsync(id, dto);

            // Push real-time: Gửi event "UpdatedVote"
            await _hubContext.Clients.All.SendAsync("UpdatedVote", new { VoteId = id, UpdatedType = dto.VoteType, UpdatedAmount = dto.Amount });

            return NoContent();
        }

        [HttpDelete("votes/{id}")]
        public async Task<IActionResult> DeleteVote(int id)
        {
            await _service.DeleteVoteAsync(id);

            // Push real-time: Gửi event "DeletedVote"
            await _hubContext.Clients.All.SendAsync("DeletedVote", id);

            return NoContent();
        }
    }
}