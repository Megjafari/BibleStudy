using BibleStudy.Application.Interfaces;
using BibleStudy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
        => await DbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id)
        => await DbSet.FindAsync(id);

    public async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }
}