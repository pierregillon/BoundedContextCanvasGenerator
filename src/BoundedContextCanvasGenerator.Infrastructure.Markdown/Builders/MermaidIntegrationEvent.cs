using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown.Builders;

public record MermaidIntegrationEvent(IntegrationEvent IntegrationEvent)
{
    private MermaidName MermaidName => new(IntegrationEvent.TypeFullName.Value, IntegrationEvent.FriendlyName);

    public IEnumerable<IMermaidGeneratable> BuildIntegrationEventNode(Node domainEventNode)
    {
        var node = Node
            .Named(MermaidName)
            .Shaped(NodeShape.Square)
            .Styled(new NodeStyleClass("integrationEvents", new NodeStyle("fill:#FFDC5C")));

        yield return node;

        yield return Link
            .From(domainEventNode)
            .To(node)
            .WithOptions(LinkOptions.Default.WithLineType(LinkLineType.Dotted));
    }
}