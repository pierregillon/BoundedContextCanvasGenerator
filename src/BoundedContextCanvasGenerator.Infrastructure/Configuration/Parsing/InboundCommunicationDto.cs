using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public record InboundCommunicationDto
{
    public string? CommandSelector { get; set; }
    public IEnumerable<CollaboratorDto>? Collaborators { get; set; }
    public IEnumerable<PolicyDto>? Policies { get; set; }
    public string? DomainEventSelector { get; set; }

    public InboundCommunicationSettings Build()
    {
        if (CommandSelector is null)
        {
            throw new InvalidOperationException("Unable to select types : selector is null");
        }

        var analyser = new PredicateAnalyser();

        return new InboundCommunicationSettings(
            TypeDefinitionPredicates.From(analyser.Analyse(CommandSelector)),
            Collaborators?
                .Where(x => x.IsNotEmpty)
                .Select(x => new CollaboratorDefinition(x.Name, TypeDefinitionPredicates.From(analyser.Analyse(x.Selector))))
                .ToArray() ?? Enumerable.Empty<CollaboratorDefinition>(),
            Policies?
                .Where(x => x.IsNotEmpty)
                .Select(x => new PolicyDefinition(new MethodAttributeMatch(x.MethodAttributePattern)))
                .ToArray() ?? Enumerable.Empty<PolicyDefinition>(),
            DomainEventSelector?
                .Pipe(analyser.Analyse)
                .Pipe(TypeDefinitionPredicates.From) 
                ?? TypeDefinitionPredicates.Empty
        );
    }
}

public class CollaboratorDto
{
    public string? Name { get; set; }
    public string? Selector { get; set; }
    public bool IsNotEmpty => Name is not null && Selector is not null;
}

public class PolicyDto
{
    public string? MethodAttributePattern { get; set; }
    public bool IsNotEmpty => MethodAttributePattern is not null;
}