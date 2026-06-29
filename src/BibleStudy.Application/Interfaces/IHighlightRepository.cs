using BibleStudy.Domain.Entities;

namespace BibleStudy.Application.Interfaces;

public interface IHighlightRepository : IGenericRepository<Highlight>
{
    Task<IEnumerable<Highlight>> GetByPassageAsync(int userId, string book, int chapter);
    Task<IEnumerable<Highlight>> GetAllByUserAsync(int userId);
}