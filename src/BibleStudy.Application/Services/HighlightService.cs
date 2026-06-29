using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Services;

public class HighlightService : IHighlightService
{
    private readonly IHighlightRepository _repository;
    private readonly ICurrentUser _currentUser;

    public HighlightService(IHighlightRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<HighlightDto>> GetAllAsync()
    {
        var highlights = await _repository.GetAllByUserAsync(_currentUser.UserId);
        return highlights.Select(MapToDto);
    }

    public async Task<HighlightDto> GetByIdAsync(int id)
    {
        var highlight = await GetOwnedHighlightAsync(id);
        return MapToDto(highlight);
    }

    public async Task<IEnumerable<HighlightDto>> GetByPassageAsync(string book, int chapter)
    {
        var highlights = await _repository.GetByPassageAsync(_currentUser.UserId, book, chapter);
        return highlights.Select(MapToDto);
    }

    public async Task<HighlightDto> CreateAsync(CreateHighlightDto dto)
    {
        Validate(dto.Color, dto.Chapter, dto.StartVerse, dto.EndVerse);

        var highlight = new Highlight
        {
            Color = dto.Color,
            Book = dto.Book,
            Chapter = dto.Chapter,
            StartVerse = dto.StartVerse,
            EndVerse = dto.EndVerse,
            CreatedAt = DateTime.UtcNow,
            UserId = _currentUser.UserId
        };

        var created = await _repository.AddAsync(highlight);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, UpdateHighlightDto dto)
    {
        Validate(dto.Color, dto.Chapter, dto.StartVerse, dto.EndVerse);

        var highlight = await GetOwnedHighlightAsync(id);
        highlight.Color = dto.Color;
        highlight.Book = dto.Book;
        highlight.Chapter = dto.Chapter;
        highlight.StartVerse = dto.StartVerse;
        highlight.EndVerse = dto.EndVerse;

        await _repository.UpdateAsync(highlight);
    }

    public async Task DeleteAsync(int id)
    {
        var highlight = await GetOwnedHighlightAsync(id);
        await _repository.DeleteAsync(highlight);
    }

    private async Task<Highlight> GetOwnedHighlightAsync(int id)
    {
        var highlight = await _repository.GetByIdAsync(id);
        if (highlight is null || highlight.UserId != _currentUser.UserId)
            throw new NotFoundException($"Highlight with id {id} was not found.");
        return highlight;
    }

    private static void Validate(string color, int chapter, int startVerse, int endVerse)
    {
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color is required.");
        if (chapter <= 0)
            throw new ArgumentException("Chapter must be greater than zero.");
        if (startVerse <= 0)
            throw new ArgumentException("Start verse must be greater than zero.");
        if (endVerse < startVerse)
            throw new ArgumentException("End verse cannot be before start verse.");
    }

    private static HighlightDto MapToDto(Highlight highlight)
        => new(highlight.Id, highlight.Color, highlight.Book, highlight.Chapter,
            highlight.StartVerse, highlight.EndVerse, highlight.CreatedAt);
}