using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _repository;

    public PlanService(IPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        var plans = await _repository.GetAllAsync();
        return plans.Select(MapToDto);
    }

    public async Task<PlanDto> GetByIdAsync(int id)
    {
        var plan = await _repository.GetByIdAsync(id);
        if (plan is null)
            throw new NotFoundException($"Plan with id {id} was not found.");

        return MapToDto(plan);
    }

    public async Task<PlanDto> CreateAsync(CreatePlanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required.");

        var plan = new Plan
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(plan);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, UpdatePlanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required.");

        var plan = await _repository.GetByIdAsync(id);
        if (plan is null)
            throw new NotFoundException($"Plan with id {id} was not found.");

        plan.Title = dto.Title;
        plan.Description = dto.Description;
        plan.StartDate = dto.StartDate;

        await _repository.UpdateAsync(plan);
    }

    public async Task DeleteAsync(int id)
    {
        var plan = await _repository.GetByIdAsync(id);
        if (plan is null)
            throw new NotFoundException($"Plan with id {id} was not found.");

        await _repository.DeleteAsync(plan);
    }

    public async Task<PlanProgressDto> GetProgressAsync(int id)
    {
        var plan = await _repository.GetByIdWithReadingsAsync(id);
        if (plan is null)
            throw new NotFoundException($"Plan with id {id} was not found.");

        var total = plan.Readings.Count;
        var read = plan.Readings.Count(r => r.IsRead);
        var percent = total == 0 ? 0 : (double)read / total * 100;

        return new PlanProgressDto(id, total, read, percent);
    }

    private static PlanDto MapToDto(Plan plan)
        => new(plan.Id, plan.Title, plan.Description, plan.StartDate, plan.CreatedAt);
}