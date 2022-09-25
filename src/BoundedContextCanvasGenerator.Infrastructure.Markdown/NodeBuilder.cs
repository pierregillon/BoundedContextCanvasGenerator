using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Markdown.Builders;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

internal class NodeBuilder
{
    private readonly DomainFlow _domainFlow;
    private readonly MermaidCommand _mermaidCommand;
    private readonly Node _commandNode;

    public NodeBuilder(DomainFlow domainFlow)
    {
        _domainFlow = domainFlow;
        _mermaidCommand = new MermaidCommand(_domainFlow.Command);
        _commandNode = _mermaidCommand.BuildCommandNode();
    }

    public IEnumerable<IMermaidGeneratable> Build()
    {
        var collaboratorNodes = BuildCollaboratorNodes();
        var policiesNodes = BuildPoliciesNode().ToArray();
        var domainEventNodes = BuildDomainEvents(policiesNodes.OfType<Node>().FirstOrDefault());

        return Enumerable
            .Empty<IMermaidGeneratable>()
            .Append(_commandNode)
            .Concat(collaboratorNodes)
            .Concat(policiesNodes)
            .Concat(domainEventNodes);
    }

    private IEnumerable<IMermaidGeneratable> BuildCollaboratorNodes()
        => _domainFlow.Collaborators.SelectMany(x => new MermaidCollaborator(x).Build(_mermaidCommand, _commandNode));

    private IEnumerable<IMermaidGeneratable> BuildPoliciesNode()
    {
        if (!_domainFlow.Policies.Any()) {
            return Enumerable.Empty<IMermaidGeneratable>();
        }

        return new MermaidPolicies(_domainFlow.Policies).Build(_mermaidCommand, _commandNode);
    }

    private IEnumerable<IMermaidGeneratable> BuildDomainEvents(Node? policiesNode)
        => _domainFlow.DomainEvents.SelectMany(x => new MermaidDomainEvent(x).Build(_commandNode, policiesNode));
}