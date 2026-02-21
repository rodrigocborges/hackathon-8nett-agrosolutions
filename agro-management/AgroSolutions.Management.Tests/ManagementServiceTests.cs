using AgroSolutions.Management.Application.DTOs;
using AgroSolutions.Management.Application.Services;
using AgroSolutions.Management.Domain.Entities;
using AgroSolutions.Management.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace AgroSolutions.Management.Tests.Services;

public class ManagementServiceTests
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IManagementService _service;

    public ManagementServiceTests()
    {
        // Criação dos Mocks (Substitutos)
        _propertyRepository = Substitute.For<IPropertyRepository>();
        _fieldRepository = Substitute.For<IFieldRepository>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();

        // Instancia o serviço com os mocks injetados
        _service = new ManagementService(_propertyRepository, _fieldRepository, _publishEndpoint);
    }

    [Fact]
    public async Task CreatePropertyAsync_DeveCriarPropriedadeComSucesso()
    {
        // Arrange
        var producerId = Guid.NewGuid();
        var request = new CreatePropertyRequest("Herdade do Sol");

        // Act
        var result = await _service.CreatePropertyAsync(request, producerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Herdade do Sol", result.Name);
        Assert.Equal(producerId, result.ProducerId);

        // Verifica se o método AddAsync foi chamado no repositório
        await _propertyRepository.Received(1).AddAsync(Arg.Any<Property>());
    }

    [Fact]
    public async Task CreateFieldAsync_DeveCriarTalhaoEPublicarEvento_QuandoPropriedadePertenceAoProdutor()
    {
        // Arrange
        var producerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var property = new Property("Herdade do Sol", producerId);

        // Simulamos que a base de dados encontra a propriedade e ela pertence ao produtor
        _propertyRepository.GetByIdAsync(propertyId).Returns(property);

        var request = new CreateFieldRequest("Milho", 50.5, propertyId);

        // Act
        var result = await _service.CreateFieldAsync(request, producerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Milho", result.CropType);
        Assert.Equal(50.5, result.AreaInHectares);

        // Verifica se persistiu o talhão
        await _fieldRepository.Received(1).AddAsync(Arg.Any<Field>());

        // Verifica se o evento FieldCreatedEvent foi publicado no RabbitMQ
        await _publishEndpoint.Received(1).Publish(Arg.Any<FieldCreatedEvent>());
    }

    [Fact]
    public async Task CreateFieldAsync_DeveLancarExcecao_QuandoPropriedadeNaoPertenceAoProdutor()
    {
        // Arrange
        var producerId = Guid.NewGuid(); // O produtor que está a fazer a requisição
        var outroProducerId = Guid.NewGuid(); // O verdadeiro dono da propriedade
        var propertyId = Guid.NewGuid();
        var property = new Property("Herdade do Vizinho", outroProducerId);

        // Simulamos que a propriedade encontrada pertence a OUTRO produtor
        _propertyRepository.GetByIdAsync(propertyId).Returns(property);

        var request = new CreateFieldRequest("Soja", 100, propertyId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.CreateFieldAsync(request, producerId));

        Assert.Equal("Propriedade não encontrada ou não pertence a este produtor.", exception.Message);

        // Garante que a segurança funcionou: não gravou na base de dados e não publicou evento
        await _fieldRepository.DidNotReceive().AddAsync(Arg.Any<Field>());
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<FieldCreatedEvent>());
    }

    [Fact]
    public async Task CreateFieldAsync_DeveLancarExcecao_QuandoPropriedadeNaoExiste()
    {
        // Arrange
        var producerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();

        // Simulamos que a propriedade não foi encontrada na base de dados
        _propertyRepository.GetByIdAsync(propertyId).Returns((Property)null!);

        var request = new CreateFieldRequest("Trigo", 30, propertyId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.CreateFieldAsync(request, producerId));
    }
}