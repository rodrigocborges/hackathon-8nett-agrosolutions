using AgroSolutions.Management.Application.DTOs;
using AgroSolutions.Management.Domain.Entities;

namespace AgroSolutions.Management.Application.Services;

public interface IManagementService
{
    Task<Property> CreatePropertyAsync(CreatePropertyRequest request, Guid producerId);
    Task<Field> CreateFieldAsync(CreateFieldRequest request, Guid producerId);
}
