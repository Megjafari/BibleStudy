using BibleStudy.Application.DTOs;

namespace BibleStudy.Application.Interfaces;

public interface IHighlightService
{
    Task<IEnumerable<HighlightDto>> GetAllAsync();
    Task<HighlightDto> GetByIdAsync(int id);
    Task<IEnumerable<HighlightDto>> GetByPassageAsync(string book, int chapter);
    Task<HighlightDto> CreateAsync(CreateHighlightDto dto);
    Task UpdateAsync(int id, UpdateHighlightDto dto);
    Task DeleteAsync(int id);
}