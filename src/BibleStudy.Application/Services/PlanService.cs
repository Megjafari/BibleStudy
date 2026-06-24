using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _repository;
    private readonly ICurrentUser _currentUser;

    public PlanService(IPlanRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        var plans = await _repository.GetAllByUserAsync(_currentUser.UserId);
        return plans.Select(MapToDto);
    }

    public async Task<PlanDto> GetByIdAsync(int id)
    {
        var plan = await GetOwnedPlanAsync(id);
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
            CreatedAt = DateTime.UtcNow,
            UserId = _currentUser.UserId
        };

        var created = await _repository.AddAsync(plan);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, UpdatePlanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required.");

        var plan = await GetOwnedPlanAsync(id);

        plan.Title = dto.Title;
        plan.Description = dto.Description;
        plan.StartDate = dto.StartDate;

        await _repository.UpdateAsync(plan);
    }

    public async Task DeleteAsync(int id)
    {
        var plan = await GetOwnedPlanAsync(id);
        await _repository.DeleteAsync(plan);
    }

    public async Task<PlanProgressDto> GetProgressAsync(int id)
    {
        var plan = await _repository.GetByIdWithReadingsAsync(id);
        if (plan is null || plan.UserId != _currentUser.UserId)
            throw new NotFoundException($"Plan with id {id} was not found.");

        var total = plan.Readings.Count;
        var read = plan.Readings.Count(r => r.IsRead);
        var percent = total == 0 ? 0 : (double)read / total * 100;

        return new PlanProgressDto(id, total, read, percent);
    }

    // Fetches a plan and ensures it belongs to the current user.
    private async Task<Plan> GetOwnedPlanAsync(int id)
    {
        var plan = await _repository.GetByIdAsync(id);
        if (plan is null || plan.UserId != _currentUser.UserId)
            throw new NotFoundException($"Plan with id {id} was not found.");

        return plan;
    }
    private static PlanDto MapToDto(Plan plan)
        => new(plan.Id, plan.Title, plan.Description, plan.StartDate, plan.CreatedAt);
}