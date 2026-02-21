using AgroSolutions.Management.Domain.Entities;
using AgroSolutions.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Management.Application.Consumers;

public class ProducerRegisteredConsumer : IConsumer<ProducerRegisteredEvent>
{
    private readonly IProducerRepository _repository;
    private readonly ILogger<ProducerRegisteredConsumer> _logger;

    public ProducerRegisteredConsumer(IProducerRepository repository, ILogger<ProducerRegisteredConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProducerRegisteredEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation($"Novo Produtor recebido no Management: {message.Name} ({message.ProducerId})");

        var exists = await _repository.ExistsAsync(message.ProducerId);
        if (!exists)
        {
            var producer = new Producer(message.ProducerId, message.Name, message.Email);
            await _repository.AddAsync(producer);
            _logger.LogInformation("Produtor salvo na base local do Management com sucesso.");
        }
    }
}