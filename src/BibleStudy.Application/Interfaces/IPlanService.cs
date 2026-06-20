using BibleStudy.Application.DTOs;

namespace BibleStudy.Application.Interfaces;

public interface IPlanService
{
    Task<IEnumerable<PlanDto>> GetAllAsync();
    Task<PlanDto> GetByIdAsync(int id);
    Task<PlanDto> CreateAsync(CreatePlanDto dto);
    Task UpdateAsync(int id, UpdatePlanDto dto);
    Task DeleteAsync(int id);
    Task<PlanProgressDto> GetProgressAsync(int id);
}