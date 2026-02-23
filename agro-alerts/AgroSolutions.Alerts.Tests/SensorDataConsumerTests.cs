using AgroSolutions.Alerts.Application.Consumers;
using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AgroSolutions.Alerts.Tests.Consumers;

public class SensorDataConsumerTests
{
    private readonly IAlertRepository _alertRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly ILogger<SensorDataConsumer> _logger;
    private readonly SensorDataConsumer _consumer;

    public SensorDataConsumerTests()
    {
        // 1. Criação dos Mocks (Substitutos das interfaces)
        _alertRepository = Substitute.For<IAlertRepository>();
        _fieldRepository = Substitute.For<IFieldRepository>();
        _logger = Substitute.For<ILogger<SensorDataConsumer>>();

        // 2. Instância do Consumer a ser testado
        _consumer = new SensorDataConsumer(_alertRepository, _fieldRepository, _logger);
    }

    [Fact]
    public async Task Consume_DeveGerarAlerta_QuandoHumidadeForCritica()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        // Simulamos o contexto da mensagem vinda do RabbitMQ
        var consumeContext = Substitute.For<ConsumeContext<SensorDataReceivedEvent>>();

        // Dados do sensor com humidade a 20% (abaixo do limite de 30%) e temperatura normal (25ºC)
        var message = new SensorDataReceivedEvent(fieldId, 20.0, 25.0, 0.0, DateTime.UtcNow);
        consumeContext.Message.Returns(message);

        // Simulamos que a base de dados encontra o talhão associado
        _fieldRepository.GetByIdAsync(fieldId).Returns(new Field(fieldId, "Soja", Guid.NewGuid()));

        // Act
        await _consumer.Consume(consumeContext);

        // Assert
        // Verifica se o repositório foi chamado para guardar um Alerta do tipo "Umidade Baixa"
        await _alertRepository.Received(1).AddAsync(Arg.Is<Alert>(a =>
            a.FieldId == fieldId &&
            a.Type == "Umidade Baixa"
        ));
    }

    [Fact]
    public async Task Consume_DeveGerarAlerta_QuandoTemperaturaForElevada()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var consumeContext = Substitute.For<ConsumeContext<SensorDataReceivedEvent>>();

        // Humidade normal (40%), mas Temperatura a 38ºC (acima do limite de 35ºC)
        var message = new SensorDataReceivedEvent(fieldId, 40.0, 38.0, 0.0, DateTime.UtcNow);
        consumeContext.Message.Returns(message);

        _fieldRepository.GetByIdAsync(fieldId).Returns(new Field(fieldId, "Milho", Guid.NewGuid()));

        // Act
        await _consumer.Consume(consumeContext);

        // Assert
        // Verifica se o repositório foi chamado para guardar um Alerta do tipo "Temperatura Alta"
        await _alertRepository.Received(1).AddAsync(Arg.Is<Alert>(a =>
            a.FieldId == fieldId &&
            a.Type == "Temperatura Alta"
        ));
    }

    [Fact]
    public async Task Consume_NaoDeveGerarNenhumAlerta_QuandoValoresEstiveremNormais()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var consumeContext = Substitute.For<ConsumeContext<SensorDataReceivedEvent>>();

        // Condições perfeitas: Humidade 50%, Temperatura 28ºC
        var message = new SensorDataReceivedEvent(fieldId, 50.0, 28.0, 0.0, DateTime.UtcNow);
        consumeContext.Message.Returns(message);

        // Act
        await _consumer.Consume(consumeContext);

        // Assert
        // O método AddAsync NÃO deve ser chamado nenhuma vez
        await _alertRepository.DidNotReceive().AddAsync(Arg.Any<Alert>());
    }
}