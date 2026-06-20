namespace BibleStudy.Application.DTOs;

public record PlanDto(
    int Id,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime CreatedAt);

public record CreatePlanDto(
    string Title,
    string? Description,
    DateTime StartDate);

public record UpdatePlanDto(
    string Title,
    string? Description,
    DateTime StartDate);

public record PlanProgressDto(
    int PlanId,
    int TotalReadings,
    int ReadReadings,
    double PercentComplete);