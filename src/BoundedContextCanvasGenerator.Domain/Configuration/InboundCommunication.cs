using BoundedContextCanvasGenerator.Infrastructure.Markdown;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record InboundCommunication(
    TypeDefinitionPredicates CommandPredicates,
    IEnumerable<CollaboratorDefinition> CollaboratorDefinitions,
    IEnumerable<PolicyDefinition> PolicyDefinitions
)
{
    public static InboundCommunication Empty => new(
        TypeDefinitionPredicates.Empty, 
        Enumerable.Empty<CollaboratorDefinition>(), 
        Enumerable.Empty<PolicyDefinition>()
    );
}