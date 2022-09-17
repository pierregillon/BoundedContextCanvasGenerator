namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record InboundCommunicationSettings(
    CommandDefinition CommandDefinition,
    IEnumerable<CollaboratorDefinition> CollaboratorDefinitions,
    IEnumerable<PolicyDefinition> PolicyDefinitions,
    TypeDefinitionPredicates DomainEventDefinitions
)
{
    public static InboundCommunicationSettings Empty => new(
        CommandDefinition.Empty, 
        Enumerable.Empty<CollaboratorDefinition>(), 
        Enumerable.Empty<PolicyDefinition>(),
        TypeDefinitionPredicates.Empty
    );
}