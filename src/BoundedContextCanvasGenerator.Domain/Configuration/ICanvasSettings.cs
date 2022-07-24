namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface ICanvasSettings
{
    public CanvasName Name { get; }
    public CanvasDefinition Definition { get; }
    public InboundCommunication InboundCommunication { get; }
    public TypeDefinitionPredicates DomainEvents { get; }
    public TypeDefinitionPredicates UbiquitousLanguage { get; }
}