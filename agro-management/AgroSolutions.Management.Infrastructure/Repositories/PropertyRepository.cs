using AgroSolutions.Management.Domain.Entities;
using AgroSolutions.Management.Domain.Interfaces;
using AgroSolutions.Management.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Management.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;
    public PropertyRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
        await _context.SaveChangesAsync();
    }

    public async Task<Property?> GetByIdAsync(Guid id) => await _context.Properties.FindAsync(id);

    public async Task<IEnumerable<Property>> GetAllByProducerIdAsync(Guid producerId)
        => await _context.Properties.Where(p => p.ProducerId == producerId).ToListAsync();
}