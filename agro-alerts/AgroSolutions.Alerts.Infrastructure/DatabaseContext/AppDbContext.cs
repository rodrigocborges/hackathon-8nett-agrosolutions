using AgroSolutions.Alerts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Alerts.Infrastructure.DatabaseContext;

public class AppDbContext : DbContext
{
    public DbSet<Field> Fields { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Field>().HasKey(f => f.Id);

        modelBuilder.Entity<Alert>().HasKey(a => a.Id);
        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Field)
            .WithMany()
            .HasForeignKey(a => a.FieldId);
    }
}