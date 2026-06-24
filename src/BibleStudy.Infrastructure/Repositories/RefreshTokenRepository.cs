using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;
using BibleStudy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
        => await Context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
}