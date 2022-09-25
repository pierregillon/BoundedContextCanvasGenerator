using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown.Builders;

public record MermaidPolicies(IEnumerable<Policy> Policies)
{
    private MermaidName MermaidNameFor(MermaidCommand command) => new(NameFor(command), FriendlyName, true);
    private static string NameFor(MermaidCommand command) => command.FullName + "Policies";
    private string FriendlyName => Policies.Select(x => x.Description).JoinWith("<br/>");

    public IEnumerable<IMermaidGeneratable> Build(MermaidCommand mermaidCommand, IMermaidLinkable commandNode)
    {
        var node = Node
            .Named(MermaidNameFor(mermaidCommand))
            .Shaped(NodeShape.Parallelogram)
            .Styled(new NodeStyleClass("policies", MermaidStyleSheet.Policy));

        yield return node;

        yield return Link
            .From(commandNode)
            .To(node)
            .WithOptions(LinkOptions.Default.WithHead(LinkHead.None));
    }
}