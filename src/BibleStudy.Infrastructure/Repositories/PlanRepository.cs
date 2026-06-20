using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;
using BibleStudy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Repositories;

public class PlanRepository : GenericRepository<Plan>, IPlanRepository
{
    public PlanRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Plan?> GetByIdWithReadingsAsync(int id)
        => await Context.Plans
            .Include(p => p.Readings)
            .FirstOrDefaultAsync(p => p.Id == id);
}