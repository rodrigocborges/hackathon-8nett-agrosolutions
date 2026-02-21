using AgroSolutions.Management.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AgroSolutions.Management.Infrastructure.DatabaseContext;

public class AppDbContext : DbContext
{
    public DbSet<Producer> Producers { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Field> Fields { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producer>().HasKey(p => p.Id);

        modelBuilder.Entity<Property>().HasKey(p => p.Id);
        modelBuilder.Entity<Property>()
            .HasOne(p => p.Producer)
            .WithMany()
            .HasForeignKey(p => p.ProducerId);

        modelBuilder.Entity<Field>().HasKey(f => f.Id);
        modelBuilder.Entity<Field>()
            .HasOne(f => f.Property)
            .WithMany()
            .HasForeignKey(f => f.PropertyId);
    }
}