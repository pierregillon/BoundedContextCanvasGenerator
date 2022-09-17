using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public record InboundCommunicationDto
{
    public CommandDefinitionDto? Commands { get; set; }
    public IEnumerable<CollaboratorDto>? Collaborators { get; set; }
    public IEnumerable<PolicyDto>? Policies { get; set; }
    public DomainEventDefinitionDto? DomainEvents { get; set; }

    public InboundCommunicationSettings Build()
    {
        if (Commands is null) {
            throw new InvalidOperationException("Unable to select types : selector is null");
        }

        return new InboundCommunicationSettings(
            ParseCommandDefinition(),
            ParseCollaboratorDefinitions(),
            ParsePolicyDefinitions(),
            ParseDomainEventDefinitions()
        );
    }

    private CommandDefinition ParseCommandDefinition()
    {
        if (Commands is null) {
            return CommandDefinition.Empty;
        }
        return new CommandDefinition(
            Commands.BuildPredicates(),
            Commands.Handler?
                .Pipe(handler =>
                    new HandlerDefinition(
                        handler.BuildPredicates(),
                        TypeDefinitionLink.Parse(handler.Link)
                    )
                ) ?? HandlerDefinition.Empty
        );
    }

    private IEnumerable<CollaboratorDefinition> ParseCollaboratorDefinitions()
    {
        return Collaborators?
            .Where(x => x.IsNotEmpty)
            .Select(x => new CollaboratorDefinition(x.Name, x.BuildPredicates()))
            .ToArray() ?? Enumerable.Empty<CollaboratorDefinition>();
    }

    private IEnumerable<PolicyDefinition> ParsePolicyDefinitions()
    {
        return Policies?
            .Where(x => x.IsNotEmpty)
            .Select(x => new PolicyDefinition(new MethodAttributeMatch(x.MethodAttributePattern)))
            .ToArray() ?? Enumerable.Empty<PolicyDefinition>();
    }

    private TypeDefinitionPredicates ParseDomainEventDefinitions() => DomainEvents?.BuildPredicates() ?? TypeDefinitionPredicates.Empty;
}