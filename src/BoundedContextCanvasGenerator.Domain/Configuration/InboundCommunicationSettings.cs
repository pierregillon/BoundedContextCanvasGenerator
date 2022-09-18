namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record InboundCommunicationSettings(CommandDefinition CommandDefinition,
    IEnumerable<CollaboratorDefinition> CollaboratorDefinitions,
    IEnumerable<PolicyDefinition> PolicyDefinitions,
    DomainEventDefinition DomainEventDefinitions, 
    IntegrationEventDefinition IntegrationEventDefinition)
{
    public static InboundCommunicationSettings Empty => new(
        CommandDefinition.Empty, 
        Enumerable.Empty<CollaboratorDefinition>(), 
        Enumerable.Empty<PolicyDefinition>(),
        DomainEventDefinition.Empty,
        IntegrationEventDefinition.Empty
    );
}