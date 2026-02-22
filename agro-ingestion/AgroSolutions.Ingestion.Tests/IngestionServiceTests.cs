using AgroSolutions.Ingestion.Application.DTOs;
using AgroSolutions.Ingestion.Application.Services;
using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;
using NSubstitute;

namespace AgroSolutions.Ingestion.Tests.Services;

public class IngestionServiceTests
{
    private readonly ISensorReadingRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IngestionService _service;

    public IngestionServiceTests()
    {
        // Criação dos Mocks
        _repository = Substitute.For<ISensorReadingRepository>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();

        // Injeção de dependência na classe de serviço
        _service = new IngestionService(_repository, _publishEndpoint);
    }

    [Fact]
    public async Task ProcessSensorDataAsync_DeveSalvarLogLocalEPublicarEvento()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var request = new SensorDataRequest(
            FieldId: fieldId,
            SoilMoisture: 28.5,
            Temperature: 32.0,
            Precipitation: 0.0
        );

        // Act
        await _service.ProcessSensorDataAsync(request);

        // Assert
        // 1. Verifica se o repositório foi chamado para salvar no SQLite
        await _repository.Received(1).AddAsync(Arg.Is<SensorReading>(r =>
            r.FieldId == fieldId &&
            r.SoilMoisture == 28.5 &&
            r.Temperature == 32.0 &&
            r.Precipitation == 0.0
        ));

        // 2. Verifica se o MassTransit publicou o evento corretamente na fila
        await _publishEndpoint.Received(1).Publish(Arg.Is<SensorDataReceivedEvent>(e =>
            e.FieldId == fieldId &&
            e.SoilMoisture == 28.5 &&
            e.Temperature == 32.0 &&
            e.Precipitation == 0.0 &&
            e.Timestamp != default // Garante que a data foi preenchida
        ));
    }
}