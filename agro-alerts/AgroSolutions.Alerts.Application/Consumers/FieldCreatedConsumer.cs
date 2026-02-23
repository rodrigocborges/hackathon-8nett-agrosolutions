using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;

namespace AgroSolutions.Alerts.Application.Consumers;

public class FieldCreatedConsumer : IConsumer<FieldCreatedEvent>
{
    private readonly IFieldRepository _repository;

    public FieldCreatedConsumer(IFieldRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<FieldCreatedEvent> context)
    {
        var msg = context.Message;
        var field = await _repository.GetByIdAsync(msg.FieldId);

        if (field == null)
        {
            await _repository.AddAsync(new Field(msg.FieldId, msg.CropType, msg.PropertyId));
        }
    }
}