namespace BoundedContextCanvasGenerator.Domain.BC.Definition;

public record CanvasDefinition(
    Text Description, 
    StrategicClassification StrategicClassification, 
    DomainRole DomainRole
)
{
    public static CanvasDefinition Empty => new(Text.Empty, StrategicClassification.Empty, DomainRole.Empty);
    public bool IsNotEmpty => !Description.IsEmpty || !StrategicClassification.IsEmpty || !DomainRole.IsEmpty;
}