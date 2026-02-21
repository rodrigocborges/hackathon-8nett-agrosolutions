namespace AgroSolutions.Management.Application.DTOs;

public record CreatePropertyRequest(string Name);
public record CreateFieldRequest(string CropType, double AreaInHectares, Guid PropertyId);