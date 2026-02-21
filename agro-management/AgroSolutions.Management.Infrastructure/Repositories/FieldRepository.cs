using AgroSolutions.Management.Domain.Entities;
using AgroSolutions.Management.Domain.Interfaces;
using AgroSolutions.Management.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Management.Infrastructure.Repositories;

public class FieldRepository : IFieldRepository
{
    private readonly AppDbContext _context;
    public FieldRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Field field)
    {
        await _context.Fields.AddAsync(field);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Field>> GetAllByPropertyIdAsync(Guid propertyId)
        => await _context.Fields.Where(f => f.PropertyId == propertyId).ToListAsync();
}
