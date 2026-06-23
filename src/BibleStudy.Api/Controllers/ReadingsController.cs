using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibleStudy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly IReadingService _readingService;

    public ReadingsController(IReadingService readingService)
    {
        _readingService = readingService;
    }

    [HttpGet("plan/{planId}")]
    public async Task<ActionResult<IEnumerable<ReadingDto>>> GetByPlan(int planId)
    {
        var readings = await _readingService.GetByPlanIdAsync(planId);
        return Ok(readings);
    }

    [HttpPost]
    public async Task<ActionResult<ReadingDto>> Create(CreateReadingDto dto)
    {
        try
        {
            var created = await _readingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByPlan), new { planId = created.PlanId }, created);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> ToggleRead(int id)
    {
        try
        {
            await _readingService.ToggleReadAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _readingService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}