using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _repository;
    private readonly ICurrentUser _currentUser;

    public NoteService(INoteRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<NoteDto>> GetAllAsync()
    {
        var notes = await _repository.GetAllByUserAsync(_currentUser.UserId);
        return notes.Select(MapToDto);
    }

    public async Task<NoteDto> GetByIdAsync(int id)
    {
        var note = await GetOwnedNoteAsync(id);
        return MapToDto(note);
    }

    public async Task<IEnumerable<NoteDto>> GetByPassageAsync(string book, int chapter)
    {
        var notes = await _repository.GetByPassageAsync(_currentUser.UserId, book, chapter);
        return notes.Select(MapToDto);
    }

    public async Task<NoteDto> CreateAsync(CreateNoteDto dto)
    {
        Validate(dto.Content, dto.Chapter, dto.StartVerse, dto.EndVerse);

        var note = new Note
        {
            Content = dto.Content,
            Book = dto.Book,
            Chapter = dto.Chapter,
            StartVerse = dto.StartVerse,
            EndVerse = dto.EndVerse,
            CreatedAt = DateTime.UtcNow,
            UserId = _currentUser.UserId
        };

        var created = await _repository.AddAsync(note);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, UpdateNoteDto dto)
    {
        Validate(dto.Content, dto.Chapter, dto.StartVerse, dto.EndVerse);

        var note = await GetOwnedNoteAsync(id);

        note.Content = dto.Content;
        note.Book = dto.Book;
        note.Chapter = dto.Chapter;
        note.StartVerse = dto.StartVerse;
        note.EndVerse = dto.EndVerse;

        await _repository.UpdateAsync(note);
    }

    public async Task DeleteAsync(int id)
    {
        var note = await GetOwnedNoteAsync(id);
        await _repository.DeleteAsync(note);
    }

    private async Task<Note> GetOwnedNoteAsync(int id)
    {
        var note = await _repository.GetByIdAsync(id);
        if (note is null || note.UserId != _currentUser.UserId)
            throw new NotFoundException($"Note with id {id} was not found.");

        return note;
    }

    private static void Validate(string content, int chapter, int startVerse, int endVerse)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.");

        if (chapter <= 0)
            throw new ArgumentException("Chapter must be greater than zero.");

        if (startVerse <= 0)
            throw new ArgumentException("Start verse must be greater than zero.");

        if (endVerse < startVerse)
            throw new ArgumentException("End verse cannot be before start verse.");
    }

    private static NoteDto MapToDto(Note note)
        => new(note.Id, note.Content, note.Book, note.Chapter,
               note.StartVerse, note.EndVerse, note.CreatedAt);
}