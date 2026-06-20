namespace BibleStudy.Application.DTOs;

public record NoteDto(
    int Id,
    string Content,
    string Book,
    int Chapter,
    int StartVerse,
    int EndVerse,
    DateTime CreatedAt);

public record CreateNoteDto(
    string Content,
    string Book,
    int Chapter,
    int StartVerse,
    int EndVerse);

public record UpdateNoteDto(
    string Content,
    string Book,
    int Chapter,
    int StartVerse,
    int EndVerse);