using BibleStudy.Application.DTOs;

namespace BibleStudy.Application.Interfaces;

public interface IReadingService
{
    Task<IEnumerable<ReadingDto>> GetByPlanIdAsync(int planId);
    Task<ReadingDto> CreateAsync(CreateReadingDto dto);
    Task ToggleReadAsync(int id);
    Task DeleteAsync(int id);
}