using System.Threading.Tasks;
using Class.Application.Services;
using Class.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Class.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TopicsController : ControllerBase
{
    private readonly TopicService _service;

    public TopicsController(TopicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var topic = await _service.GetByIdAsync(id);
        return topic == null ? NotFound() : Ok(topic);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTopicDto dto)
    {
        var createdTopic = await _service.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdTopic.TopicId }, createdTopic);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTopicDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}