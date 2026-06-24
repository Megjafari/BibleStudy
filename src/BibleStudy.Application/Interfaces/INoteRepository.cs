using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Interfaces;

public interface INoteRepository : IGenericRepository<Note>
{
    Task<IEnumerable<Note>> GetByPassageAsync(int userId, string book, int chapter);
    Task<IEnumerable<Note>> GetAllByUserAsync(int userId);
}