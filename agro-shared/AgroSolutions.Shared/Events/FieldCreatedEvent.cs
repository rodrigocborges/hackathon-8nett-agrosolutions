namespace AgroSolutions.Shared.Events;

public record FieldCreatedEvent(Guid FieldId, string CropType, Guid PropertyId);