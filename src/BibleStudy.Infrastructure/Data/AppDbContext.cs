using BibleStudy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibleStudy.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Reading> Readings => Set<Reading>();
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plan>()
            .HasMany(p => p.Readings)
            .WithOne(r => r.Plan)
            .HasForeignKey(r => r.PlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}