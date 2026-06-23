namespace BibleStudy.Application.DTOs;

public record ReadingDto(
    int Id,
    int PlanId,
    string Book,
    int StartChapter,
    int EndChapter,
    int DayNumber,
    bool IsRead,
    DateTime? ReadAt);

public record CreateReadingDto(
    int PlanId,
    string Book,
    int StartChapter,
    int EndChapter,
    int DayNumber);