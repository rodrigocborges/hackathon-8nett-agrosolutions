namespace AgroSolutions.Shared.Events;

public record ProducerRegisteredEvent(Guid ProducerId, string Name, string Email);