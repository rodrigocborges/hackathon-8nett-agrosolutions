using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Domain.Entities;

namespace AgroSolutions.Identity.Application.Services;

public interface IProducerService
{
    Task<Producer> RegisterAsync(RegisterRequest request);
    Task<Producer?> ValidateLoginAsync(LoginRequest request);
}
