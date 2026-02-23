using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Alerts.Infrastructure.DatabaseContext;

namespace AgroSolutions.Alerts.Infrastructure.Repositories;

public class FieldRepository : IFieldRepository
{
    private readonly AppDbContext _context;
    public FieldRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Field field)
    {
        await _context.Fields.AddAsync(field);
        await _context.SaveChangesAsync();
    }

    public async Task<Field?> GetByIdAsync(Guid id) => await _context.Fields.FindAsync(id);
}