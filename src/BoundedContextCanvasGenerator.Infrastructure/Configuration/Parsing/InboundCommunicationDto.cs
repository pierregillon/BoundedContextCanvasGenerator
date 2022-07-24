using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public record InboundCommunicationDto
{
    public string? Selector { get; set; }

    public InboundCommunication Build()
    {
        if (Selector is null)
        {
            throw new InvalidOperationException("Unable to select types : selector is null");
        }
        return new InboundCommunication(
            TypeDefinitionPredicates.From(new PredicateAnalyser().Analyse(Selector)),
            Enumerable.Empty<CollaboratorDefinition>(),
            Enumerable.Empty<PolicyDefinition>()
        );
    }
}