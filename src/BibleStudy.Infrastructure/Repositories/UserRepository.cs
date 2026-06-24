using BibleStudy.Application.Interfaces;
using BibleStudy.Domain.Entities;
using BibleStudy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
        => await Context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> ExistsByEmailAsync(string email)
        => await Context.Users.AnyAsync(u => u.Email == email);

    public async Task<bool> ExistsByUsernameAsync(string username)
        => await Context.Users.AnyAsync(u => u.Username == username);
}