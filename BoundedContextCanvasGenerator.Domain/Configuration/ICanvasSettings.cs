namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface ICanvasSettings
{
    public TypeDefinitionPredicates Commands { get; }
    public TypeDefinitionPredicates DomainEvents { get; }
}