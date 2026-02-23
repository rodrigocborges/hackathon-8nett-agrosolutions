using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Alerts.Application.Consumers;

public class SensorDataConsumer : IConsumer<SensorDataReceivedEvent>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly ILogger<SensorDataConsumer> _logger;

    // Regras de negócio do MVP
    private const double MIN_MOISTURE = 30.0;
    private const double MAX_TEMP = 35.0;

    public SensorDataConsumer(IAlertRepository alertRepository, IFieldRepository fieldRepository, ILogger<SensorDataConsumer> logger)
    {
        _alertRepository = alertRepository;
        _fieldRepository = fieldRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SensorDataReceivedEvent> context)
    {
        var data = context.Message;
        var field = await _fieldRepository.GetByIdAsync(data.FieldId);

        // Regra 1: Umidade Crítica
        if (data.SoilMoisture < MIN_MOISTURE)
        {
            var msg = $"Alerta Crítico: Umidade do solo em {data.SoilMoisture}%. Nível abaixo de {MIN_MOISTURE}%. Ação imediata de irrigação necessária.";
            await _alertRepository.AddAsync(new Alert(data.FieldId, "Umidade Baixa", msg));
            _logger.LogWarning(msg);
        }

        // Regra 2: Temperatura Elevada
        if (data.Temperature > MAX_TEMP)
        {
            var msg = $"Aviso: Temperatura atingiu {data.Temperature}°C. Risco de estresse hídrico na cultura {(field?.CropType ?? "desconhecida")}.";
            await _alertRepository.AddAsync(new Alert(data.FieldId, "Temperatura Alta", msg));
            _logger.LogWarning(msg);
        }
    }
}