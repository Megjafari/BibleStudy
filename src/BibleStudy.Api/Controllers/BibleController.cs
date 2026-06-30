using BibleStudy.Application.DTOs;
using BibleStudy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibleStudy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BibleController : ControllerBase
{
    private readonly IBibleTextProvider _bibleTextProvider;

    public BibleController(IBibleTextProvider bibleTextProvider)
    {
        _bibleTextProvider = bibleTextProvider;
    }

    [HttpGet("books")]
    public async Task<ActionResult<IEnumerable<BibleBookDto>>> GetBooks(
        [FromQuery] string translation = "kjv")
    {
        try
        {
            var result = await _bibleTextProvider.GetBooksAsync(translation);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, ex.Message);
        }
    }

    [HttpGet("books/{book}/chapters")]
    public async Task<ActionResult<IEnumerable<int>>> GetChapters(
        string book, [FromQuery] string translation = "kjv")
    {
        try
        {
            var result = await _bibleTextProvider.GetChaptersAsync(book, translation);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, ex.Message);
        }
    }

    [HttpGet("{book}/{chapter}")]
    public async Task<ActionResult<BibleChapterDto>> GetChapter(
        string book, int chapter, [FromQuery] string translation = "kjv")
    {
        try
        {
            var result = await _bibleTextProvider.GetChapterAsync(book, chapter, translation);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, ex.Message);
        }
    }
}