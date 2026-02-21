using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Application.Services;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using MassTransit;
using NSubstitute;

namespace AgroSolutions.Identity.Tests.Services;

public class ProducerServiceTests
{
    private readonly IProducerRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ProducerService _service;

    public ProducerServiceTests()
    {
        _repository = Substitute.For<IProducerRepository>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _service = new ProducerService(_repository, _publishEndpoint);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateProducer_WhenEmailIsUnique()
    {
        // Arrange
        var request = new RegisterRequest("Fazendeiro Alfredinho", "alfredinho@agro.com", "Senha123");
        _repository.GetByEmailAsync(request.Email).Returns((Producer)null!);

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);

        // Verifica se persistiu no banco
        await _repository.Received(1).AddAsync(Arg.Any<Producer>());

        // Verifica se o evento foi disparado para o RabbitMQ
        await _publishEndpoint.Received(1).Publish(Arg.Any<ProducerRegisteredEvent>());
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest("Outro", "jaexiste@agro.com", "123");
        var existing = new Producer("Nome", request.Email, "hash");
        _repository.GetByEmailAsync(request.Email).Returns(existing);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(request));
    }
}