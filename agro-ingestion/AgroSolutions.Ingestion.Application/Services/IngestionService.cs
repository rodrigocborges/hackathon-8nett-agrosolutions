using AgroSolutions.Ingestion.Application.DTOs;
using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;

namespace AgroSolutions.Ingestion.Application.Services;

public class IngestionService : IIngestionService
{
    private readonly ISensorReadingRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public IngestionService(ISensorReadingRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task ProcessSensorDataAsync(SensorDataRequest request)
    {
        // 1. Salva um log rápido no banco local para rastreabilidade
        var reading = new SensorReading(request.FieldId, request.SoilMoisture, request.Temperature, request.Precipitation);
        await _repository.AddAsync(reading);

        // 2. Dispara o evento para o RabbitMQ
        // Este é o momento onde a arquitetura brilha: a API responde rápido, e os workers processam depois.
        var eventMessage = new SensorDataReceivedEvent(
            request.FieldId,
            request.SoilMoisture,
            request.Temperature,
            request.Precipitation,
            reading.ReceivedAt
        );

        await _publishEndpoint.Publish(eventMessage);
    }
}