using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Interfaces;

public interface IReadingRepository : IGenericRepository<Reading>
{
    Task<IEnumerable<Reading>> GetByPlanIdAsync(int planId);
}