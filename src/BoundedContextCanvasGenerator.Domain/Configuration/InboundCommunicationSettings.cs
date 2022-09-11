namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record InboundCommunicationSettings(
    TypeDefinitionPredicates CommandPredicates,
    IEnumerable<CollaboratorDefinition> CollaboratorDefinitions,
    IEnumerable<PolicyDefinition> PolicyDefinitions,
    TypeDefinitionPredicates DomainEventDefinitions
)
{
    public static InboundCommunicationSettings Empty => new(
        TypeDefinitionPredicates.Empty, 
        Enumerable.Empty<CollaboratorDefinition>(), 
        Enumerable.Empty<PolicyDefinition>(),
        TypeDefinitionPredicates.Empty
    );
}