using AgroSolutions.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Identity.Infrastructure.DatabaseContext;

public class AppDbContext : DbContext
{
    public DbSet<Producer> Producers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producer>().HasKey(p => p.Id);
        modelBuilder.Entity<Producer>().HasIndex(p => p.Email).IsUnique();
    }
}