using BibleStudy.Application.DTOs;
using BibleStudy.Application.Exceptions;
using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Services;

public class ReadingService : IReadingService
{
    private readonly IReadingRepository _readingRepository;
    private readonly IPlanRepository _planRepository;

    public ReadingService(IReadingRepository readingRepository, IPlanRepository planRepository)
    {
        _readingRepository = readingRepository;
        _planRepository = planRepository;
    }

    public async Task<IEnumerable<ReadingDto>> GetByPlanIdAsync(int planId)
    {
        var readings = await _readingRepository.GetByPlanIdAsync(planId);
        return readings.Select(MapToDto);
    }

    public async Task<ReadingDto> CreateAsync(CreateReadingDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Book))
            throw new ArgumentException("Book is required.");

        if (dto.StartChapter <= 0 || dto.EndChapter <= 0)
            throw new ArgumentException("Chapters must be greater than zero.");

        if (dto.EndChapter < dto.StartChapter)
            throw new ArgumentException("End chapter cannot be before start chapter.");

        // Make sure the plan exists before attaching a reading to it.
        var plan = await _planRepository.GetByIdAsync(dto.PlanId);
        if (plan is null)
            throw new NotFoundException($"Plan with id {dto.PlanId} was not found.");

        var reading = new Reading
        {
            PlanId = dto.PlanId,
            Book = dto.Book,
            StartChapter = dto.StartChapter,
            EndChapter = dto.EndChapter,
            DayNumber = dto.DayNumber,
            IsRead = false
        };

        var created = await _readingRepository.AddAsync(reading);
        return MapToDto(created);
    }

    public async Task ToggleReadAsync(int id)
    {
        var reading = await _readingRepository.GetByIdAsync(id);
        if (reading is null)
            throw new NotFoundException($"Reading with id {id} was not found.");

        reading.IsRead = !reading.IsRead;
        reading.ReadAt = reading.IsRead ? DateTime.UtcNow : null;

        await _readingRepository.UpdateAsync(reading);
    }

    public async Task DeleteAsync(int id)
    {
        var reading = await _readingRepository.GetByIdAsync(id);
        if (reading is null)
            throw new NotFoundException($"Reading with id {id} was not found.");

        await _readingRepository.DeleteAsync(reading);
    }

    private static ReadingDto MapToDto(Reading r)
        => new(r.Id, r.PlanId, r.Book, r.StartChapter, r.EndChapter,
               r.DayNumber, r.IsRead, r.ReadAt);
}