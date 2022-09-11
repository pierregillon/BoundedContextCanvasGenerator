using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

internal record Nodes(Node _commandNode, IEnumerable<Node> _collaboratorNodes, Node? _policiesNode, IEnumerable<Node> _domainEventNodes)
{
    private readonly Node _commandNode = _commandNode;
    private readonly IEnumerable<Node> _collaboratorNodes = _collaboratorNodes;
    private readonly Node? _policiesNode = _policiesNode;
    private readonly IEnumerable<Node> _domainEventNodes = _domainEventNodes;

    public IEnumerable<IMermaidGeneratable> GetAll()
    {
        foreach (var node in GetAllNodes())
        {
            yield return node;
        }

        foreach (var link in BuildLinks())
        {
            yield return link;
        }
    }

    private IEnumerable<Node> GetAllNodes()
    {
        yield return _commandNode;

        foreach (var collaborator in _collaboratorNodes)
        {
            yield return collaborator;
        }

        if (_policiesNode is not null)
        {
            yield return _policiesNode;
        }

        foreach (var domainEventNode in _domainEventNodes)
        {
            yield return domainEventNode;
        }
    }

    private IEnumerable<Link> BuildLinks()
    {
        foreach (var collaboratorNode in _collaboratorNodes)
        {
            yield return Link.From(collaboratorNode).To(_commandNode);
        }

        if (_policiesNode is not null)
        {
            yield return Link
                .From(_commandNode)
                .To(_policiesNode)
                .WithOptions(LinkOptions.Default.WithHead(LinkHead.None));
        }

        foreach (var domainEventNode in _domainEventNodes)
        {
            yield return Link
                .From(_policiesNode ?? _commandNode)
                .To(domainEventNode)
                .WithOptions(LinkOptions.Default.WithLineType(LinkLineType.Dotted));
        }
    }
}