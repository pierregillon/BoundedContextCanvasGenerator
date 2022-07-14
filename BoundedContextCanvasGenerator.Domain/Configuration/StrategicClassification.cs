namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record StrategicClassification(DomainType DomainType, BusinessModel BusinessModel, Evolution Evolution)
{
    public static StrategicClassification Empty => new(DomainType.Unknown, BusinessModel.Unknown, Evolution.Unknown);
    public bool IsEmpty => this == Empty;
}