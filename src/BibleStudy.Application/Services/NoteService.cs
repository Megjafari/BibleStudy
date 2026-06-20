using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _repository;

    public NoteService(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NoteDto>> GetAllAsync()
    {
        var notes = await _repository.GetAllAsync();
        return notes.Select(MapToDto);
    }

    public async Task<NoteDto> GetByIdAsync(int id)
    {
        var note = await _repository.GetByIdAsync(id);
        if (note is null)
            throw new NotFoundException($"Note with id {id} was not found.");

        return MapToDto(note);
    }

    public async Task<IEnumerable<NoteDto>> GetByPassageAsync(string book, int chapter)
    {
        var notes = await _repository.GetByPassageAsync(book, chapter);
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
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(note);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, UpdateNoteDto dto)
    {
        Validate(dto.Content, dto.Chapter, dto.StartVerse, dto.EndVerse);

        var note = await _repository.GetByIdAsync(id);
        if (note is null)
            throw new NotFoundException($"Note with id {id} was not found.");

        note.Content = dto.Content;
        note.Book = dto.Book;
        note.Chapter = dto.Chapter;
        note.StartVerse = dto.StartVerse;
        note.EndVerse = dto.EndVerse;

        await _repository.UpdateAsync(note);
    }

    public async Task DeleteAsync(int id)
    {
        var note = await _repository.GetByIdAsync(id);
        if (note is null)
            throw new NotFoundException($"Note with id {id} was not found.");

        await _repository.DeleteAsync(note);
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