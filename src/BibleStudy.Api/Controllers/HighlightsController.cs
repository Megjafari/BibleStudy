using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BibleStudy.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HighlightsController : ControllerBase
{
    private readonly IHighlightService _highlightService;

    public HighlightsController(IHighlightService highlightService)
    {
        _highlightService = highlightService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HighlightDto>>> GetAll(
        [FromQuery] string? book, [FromQuery] int? chapter)
    {
        if (book is not null && chapter is not null)
        {
            var filtered = await _highlightService.GetByPassageAsync(book, chapter.Value);
            return Ok(filtered);
        }

        var highlights = await _highlightService.GetAllAsync();
        return Ok(highlights);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HighlightDto>> GetById(int id)
    {
        try
        {
            var highlight = await _highlightService.GetByIdAsync(id);
            return Ok(highlight);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<HighlightDto>> Create(CreateHighlightDto dto)
    {
        try
        {
            var created = await _highlightService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateHighlightDto dto)
    {
        try
        {
            await _highlightService.UpdateAsync(id, dto);
            return NoContent();
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _highlightService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}