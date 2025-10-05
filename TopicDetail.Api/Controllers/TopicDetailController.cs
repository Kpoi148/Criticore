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
        private readonly IHubContext<TopicHub> _hubContext;

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
            await _hubContext.Clients.All.SendAsync("NewAnswer", created);
            return CreatedAtAction(nameof(GetAnswerById), new { id = created.AnswerId }, created);
        }

        [HttpPut("answers/{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] UpdateAnswerDto dto)
        {
            await _service.UpdateAnswerAsync(id, dto);
            await _hubContext.Clients.All.SendAsync("UpdatedAnswer", new { AnswerId = id, UpdatedContent = dto.Content });
            return NoContent();
        }

        [HttpDelete("answers/{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            await _service.DeleteAnswerAsync(id);
            await _hubContext.Clients.All.SendAsync("DeletedAnswer", id);
            return NoContent();
        }

        // CRUD for Vote
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
            var existingVote = await _service.GetVoteByAnswerAndUserAsync(dto.AnswerId, dto.UserId);
            VoteDto result;

            if (existingVote != null)
            {
                var updateDto = new UpdateVoteDto { VoteType = dto.VoteType, Amount = dto.Amount };
                await _service.UpdateVoteAsync(existingVote.VoteId, updateDto);
                result = await _service.GetVoteByIdAsync(existingVote.VoteId);
            }
            else
            {
                result = await _service.CreateVoteAsync(dto);
            }

            // Tính new average sau thay đổi
            var answer = await _service.GetAnswerByIdAsync(dto.AnswerId);
            if (answer == null) return NotFound();

            var signalRData = new
            {
                VoteId = result.VoteId,
                AnswerId = dto.AnswerId,
                NewAverage = answer.Rating,
                NewVoteCount = answer.VoteCount // Giả sử đã thêm vào DTO
            };

            // Send event phù hợp
            var eventName = existingVote != null ? "UpdatedVote" : "CreatedVote";
            await _hubContext.Clients.All.SendAsync(eventName, signalRData);

            return existingVote != null ? Ok(result) : CreatedAtAction(nameof(GetVoteById), new { id = result.VoteId }, result);
        }

        [HttpPut("votes/{id}")]
        public async Task<IActionResult> UpdateVote(int id, [FromBody] UpdateVoteDto dto)
        {
            var existingVote = await _service.GetVoteByIdAsync(id);
            if (existingVote == null) return NotFound();

            await _service.UpdateVoteAsync(id, dto);

            // Tính new average
            var answer = await _service.GetAnswerByIdAsync(existingVote.AnswerId);
            if (answer == null) return NotFound();

            var signalRData = new
            {
                VoteId = id,
                AnswerId = existingVote.AnswerId,
                NewAverage = answer.Rating,
                NewVoteCount = answer.VoteCount
            };

            await _hubContext.Clients.All.SendAsync("UpdatedVote", signalRData);
            return NoContent();
        }

        [HttpDelete("votes/{id}")]
        public async Task<IActionResult> DeleteVote(int id)
        {
            var vote = await _service.GetVoteByIdAsync(id);
            if (vote == null) return NotFound();

            await _service.DeleteVoteAsync(id);

            // Tính new average
            var answer = await _service.GetAnswerByIdAsync(vote.AnswerId);
            if (answer == null) return NotFound();

            var signalRData = new
            {
                VoteId = id,
                AnswerId = vote.AnswerId,
                NewAverage = answer.Rating,
                NewVoteCount = answer.VoteCount
            };

            await _hubContext.Clients.All.SendAsync("DeletedVote", signalRData);
            return NoContent();
        }
    }
}