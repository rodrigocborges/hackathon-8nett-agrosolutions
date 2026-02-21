using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Interfaces;
using AgroSolutions.Shared.Events;
using AgroSolutions.Shared.Utils;
using MassTransit;

namespace AgroSolutions.Identity.Application.Services;

public class ProducerService : IProducerService
{
    private readonly IProducerRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProducerService(IProducerRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Producer> RegisterAsync(RegisterRequest request)
    {
        if (!EmailUtil.IsValid(request.Email))
            throw new InvalidOperationException("E-mail inválido.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new InvalidOperationException("A senha deve ter no mínimo 6 caracteres!");

        var existingProducer = await _repository.GetByEmailAsync(request.Email);
        if (existingProducer != null)
            throw new InvalidOperationException("E-mail já cadastrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var newProducer = new Producer(request.Name, request.Email, passwordHash);

        await _repository.AddAsync(newProducer);

        // Dispara o evento pro RabbitMQ avisando os outros microsserviços!
        await _publishEndpoint.Publish(new ProducerRegisteredEvent(newProducer.Id, newProducer.Name, newProducer.Email));

        return newProducer;
    }

    public async Task<Producer?> ValidateLoginAsync(LoginRequest request)
    {
        if (!EmailUtil.IsValid(request.Email))
            throw new InvalidOperationException("E-mail inválido.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new InvalidOperationException("Senha inválida!");

        var producer = await _repository.GetByEmailAsync(request.Email);
        if (producer == null || !BCrypt.Net.BCrypt.Verify(request.Password, producer.PasswordHash))
            return null;

        return producer;
    }
}