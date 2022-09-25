using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown.Builders;

public record MermaidCollaborator(Collaborator Collaborator)
{
    private MermaidName MermaidNameFor(MermaidCommand command) => new(NameFor(command), FriendlyName);
    private string NameFor(MermaidCommand command) => command.FullName + MermaidName;
    private string MermaidName => Collaborator.Name.ToPascalCase() + "Collaborator";
    private string FriendlyName => Collaborator.Name;

    public IEnumerable<IMermaidGeneratable> Build(MermaidCommand mermaidCommand, IMermaidLinkable commandNode)
    {
        var collaboratorNode = Node
            .Named(MermaidNameFor(mermaidCommand))
            .Shaped(NodeShape.Asymmetric)
            .Styled(GetNodeStyle());

        yield return collaboratorNode;
        yield return Link.From(collaboratorNode).To(commandNode);
    }

    private NodeStyleClass GetNodeStyle()
    {
        return Collaborator.Type switch
        {
            CollaboratorType.Front => new NodeStyleClass("frontCollaborators", new NodeStyle("fill:#FFE5FF")),
            CollaboratorType.BoundedContext => new NodeStyleClass("boundedContextCollaborators", new NodeStyle("fill:#FF5C5C")),
            _ => throw new InvalidOperationException("Unknown collaborator: enable to choose node style")
        };
    }
}