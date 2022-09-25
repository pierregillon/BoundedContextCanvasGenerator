using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown.Builders;

public record MermaidDomainEvent(DomainEvent DomainEvent)
{
    private MermaidName MermaidName => new(DomainEvent.TypeFullName.Value, DomainEvent.FriendlyName);

    public IEnumerable<IMermaidGeneratable> Build(IMermaidLinkable commandNode, Node? policiesNode)
    {
        var domainEventNode = Node
            .Named(MermaidName)
            .Shaped(NodeShape.Square)
            .Styled(new NodeStyleClass("domainEvents", MermaidStyleSheet.DomainEvent));

        yield return domainEventNode;
        yield return Link
            .From(policiesNode ?? commandNode)
            .To(domainEventNode)
            .WithOptions(LinkOptions.Default.WithLineType(LinkLineType.Dotted));

        foreach (var mermaidGeneratable in DomainEvent.IntegrationEvents.SelectMany(x => new MermaidIntegrationEvent(x).BuildIntegrationEventNode(domainEventNode))) {
            yield return mermaidGeneratable;
        }
    }
}