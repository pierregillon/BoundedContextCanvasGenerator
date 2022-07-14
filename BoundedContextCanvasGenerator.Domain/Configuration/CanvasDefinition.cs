namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record CanvasDefinition(
    Text Description, 
    StrategicClassification StrategicClassification, 
    DomainRole DomainRole
)
{
    public static CanvasDefinition Empty => new(Text.Empty, StrategicClassification.Empty, DomainRole.Empty);
    public bool IsEnabled => !Description.IsEmpty || !StrategicClassification.IsEmpty || !DomainRole.IsEmpty;
}