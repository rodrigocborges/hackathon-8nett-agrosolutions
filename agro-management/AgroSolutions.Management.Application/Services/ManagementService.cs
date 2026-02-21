using AgroSolutions.Management.Application.DTOs;
using AgroSolutions.Management.Domain.Entities;
using AgroSolutions.Management.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;

namespace AgroSolutions.Management.Application.Services;

public class ManagementService : IManagementService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ManagementService(IPropertyRepository propertyRepository, IFieldRepository fieldRepository, IPublishEndpoint publishEndpoint)
    {
        _propertyRepository = propertyRepository;
        _fieldRepository = fieldRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Property> CreatePropertyAsync(CreatePropertyRequest request, Guid producerId)
    {
        var property = new Property(request.Name, producerId);
        await _propertyRepository.AddAsync(property);
        return property;
    }

    public async Task<Field> CreateFieldAsync(CreateFieldRequest request, Guid producerId)
    {
        // Valida se a propriedade existe e se pertence ao produtor logado
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null || property.ProducerId != producerId)
            throw new UnauthorizedAccessException("Propriedade não encontrada ou não pertence a este produtor.");

        var field = new Field(request.CropType, request.AreaInHectares, request.PropertyId);
        await _fieldRepository.AddAsync(field);

        // Avisa a arquitetura que um novo Talhão nasceu!
        await _publishEndpoint.Publish(new FieldCreatedEvent(field.Id, field.CropType, field.PropertyId));

        return field;
    }
}