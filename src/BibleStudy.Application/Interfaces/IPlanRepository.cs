using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Interfaces;

public interface IPlanRepository : IGenericRepository<Plan>
{
    Task<Plan?> GetByIdWithReadingsAsync(int id);
}