using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Interfaces;

public interface INoteRepository : IGenericRepository<Note>
{
    Task<IEnumerable<Note>> GetByPassageAsync(string book, int chapter);
}