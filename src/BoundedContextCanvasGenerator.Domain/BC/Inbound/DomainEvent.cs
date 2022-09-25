using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record DomainEvent(string FriendlyName, TypeFullName TypeFullName, IEnumerable<IntegrationEvent> IntegrationEvents)
{
    public IEnumerable<IntegrationEvent> IntegrationEvents = IntegrationEvents;

    public static DomainEvent FromType(TypeDefinition typeDefinition) 
        => new(typeDefinition.FullName.Name.ToReadableSentence(), typeDefinition.FullName, Enumerable.Empty<IntegrationEvent>());
}

public record IntegrationEvent(string FriendlyName, TypeFullName TypeFullName)
{
    public static IntegrationEvent FromType(TypeDefinition typeDefinition)
        => new(typeDefinition.FullName.Name.ToReadableSentence(), typeDefinition.FullName);
}