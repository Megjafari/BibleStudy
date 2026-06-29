namespace BibleStudy.Application.DTOs;

public record HighlightDto(
    int Id,
    string Color,
    string Book,
    int Chapter,
    int StartVerse,
    int EndVerse,
    DateTime CreatedAt);

public record CreateHighlightDto(
    string Color,
    string Book,
    int Chapter,
    int StartVerse,
    int EndVerse);

public record UpdateHighlightDto(
    string Color,
    string Book,
    int Chapter,
    int StartVerse,
    int EndVerse);