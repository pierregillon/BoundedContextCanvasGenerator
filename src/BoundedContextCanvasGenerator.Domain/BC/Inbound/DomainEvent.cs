using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record DomainEvent(string FriendlyName, TypeFullName TypeFullName, IEnumerable<IntegrationEvent> IntegrationEvents)
{
    private const string DOMAIN_EVENT_SUFFIX = "DomainEvent";

    public IEnumerable<IntegrationEvent> IntegrationEvents = IntegrationEvents;

    public static DomainEvent FromType(TypeDefinition typeDefinition) 
        => new(typeDefinition.FullName.Name.TrimWord(DOMAIN_EVENT_SUFFIX).ToReadableSentence(), typeDefinition.FullName, Enumerable.Empty<IntegrationEvent>());
}