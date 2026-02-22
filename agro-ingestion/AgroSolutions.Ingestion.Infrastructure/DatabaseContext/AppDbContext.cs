using AgroSolutions.Ingestion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AgroSolutions.Ingestion.Infrastructure.DatabaseContext;

public class AppDbContext : DbContext
{
    public DbSet<SensorReading> SensorReadings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorReading>().HasKey(s => s.Id);
    }
}