using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;
using BibleStudy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Repositories;

public class ReadingRepository : GenericRepository<Reading>, IReadingRepository
{
    public ReadingRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reading>> GetByPlanIdAsync(int planId)
        => await Context.Readings
            .Where(r => r.PlanId == planId)
            .OrderBy(r => r.DayNumber)
            .ToListAsync();
}