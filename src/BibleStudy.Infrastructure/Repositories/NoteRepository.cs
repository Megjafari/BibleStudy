using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;
using BibleStudy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Repositories;

public class NoteRepository : GenericRepository<Note>, INoteRepository
{
    public NoteRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Note>> GetByPassageAsync(string book, int chapter)
        => await Context.Notes
            .Where(n => n.Book == book && n.Chapter == chapter)
            .ToListAsync();
}