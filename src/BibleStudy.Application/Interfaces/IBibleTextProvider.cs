using BibleStudy.Application.DTOs;

namespace BibleStudy.Application.Interfaces;

public interface IBibleTextProvider
{
    Task<BibleChapterDto> GetChapterAsync(string book, int chapter, string translation);
}