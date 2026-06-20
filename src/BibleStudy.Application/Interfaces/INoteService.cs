using BibleStudy.Application.DTOs;

namespace BibleStudy.Application.Interfaces;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllAsync();
    Task<NoteDto> GetByIdAsync(int id);
    Task<IEnumerable<NoteDto>> GetByPassageAsync(string book, int chapter);
    Task<NoteDto> CreateAsync(CreateNoteDto dto);
    Task UpdateAsync(int id, UpdateNoteDto dto);
    Task DeleteAsync(int id);
}