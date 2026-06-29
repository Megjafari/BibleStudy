using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;
using BibleStudy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Repositories;

public class HighlightRepository : GenericRepository<Highlight>, IHighlightRepository
{
    public HighlightRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Highlight>> GetByPassageAsync(int userId, string book, int chapter)
        => await Context.Highlights
            .Where(h => h.UserId == userId && h.Book == book && h.Chapter == chapter)
            .ToListAsync();

    public async Task<IEnumerable<Highlight>> GetAllByUserAsync(int userId)
        => await Context.Highlights
            .Where(h => h.UserId == userId)
            .ToListAsync();
}