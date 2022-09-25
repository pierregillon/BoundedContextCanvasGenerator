using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown.Builders;

public record MermaidCommand(Command Command)
{
    public string FullName => Command.TypeFullName.Value;
    private MermaidName MermaidName => new(FullName, Command.FriendlyName);

    public Node BuildCommandNode()
        => Node.Named(MermaidName);
}