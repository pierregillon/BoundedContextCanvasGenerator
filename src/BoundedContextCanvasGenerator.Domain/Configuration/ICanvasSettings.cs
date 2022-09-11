using BoundedContextCanvasGenerator.Domain.BC.Definition;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface ICanvasSettings
{
    public CanvasName Name { get; }
    public CanvasDefinition Definition { get; }
    public InboundCommunicationSettings InboundCommunicationSettings { get; }
    public TypeDefinitionPredicates UbiquitousLanguage { get; }
}