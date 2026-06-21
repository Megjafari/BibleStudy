namespace BibleStudy.Application.DTOs;

public record BibleVerseDto(int Verse, string Text);

public record BibleChapterDto(
    string Reference,
    string Book,
    int Chapter,
    string Translation,
    string Copyright,
    IEnumerable<BibleVerseDto> Verses);