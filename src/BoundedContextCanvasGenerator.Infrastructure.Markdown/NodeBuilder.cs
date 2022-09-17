using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

internal class NodeBuilder
{
    private readonly DomainFlow _domainFlow;
    private readonly MermaidCommand _mermaidCommand;

    public NodeBuilder(DomainFlow domainFlow)
    {
        _domainFlow = domainFlow;
        _mermaidCommand = new MermaidCommand(_domainFlow.Command);
    }

    public IEnumerable<IMermaidGeneratable> BuildAll()
        => new Nodes(
            BuildCommandNode(),
            BuildCollaboratorNodes(),
            BuildPoliciesNode(),
            BuildDomainEventNodes()
        ).GetAll();

    private Node BuildCommandNode()
        => Node.Named(_mermaidCommand.MermaidName);

    private IEnumerable<Node> BuildCollaboratorNodes()
        => _domainFlow.Collaborators
            .Select(x => new MermaidCollaborator(x))
            .Select(BuildCollaboratorNode)
            .ToArray();

    private Node? BuildPoliciesNode()
        => _domainFlow.Policies.Any()
            ? BuildPolicyNode(new MermaidPolicies(_domainFlow.Policies))
            : null;

    private IEnumerable<Node> BuildDomainEventNodes()
        => _domainFlow.DomainEvents
            .Select(x => new MermaidDomainEvent(x))
            .Select(BuildDomainEventNode)
            .ToArray();

    private Node BuildCollaboratorNode(MermaidCollaborator mermaidCollaborator)
        => Node
            .Named(mermaidCollaborator.MermaidNameFor(_mermaidCommand))
            .Shaped(NodeShape.Asymmetric)
            .Styled(mermaidCollaborator.GetNodeStyle());

    private Node BuildPolicyNode(MermaidPolicies policies)
        => Node
            .Named(policies.MermaidNameFor(_mermaidCommand))
            .Shaped(NodeShape.Parallelogram)
            .Styled(new NodeStyleClass("policies", new NodeStyle("fill:#FFFFAD, font-style:italic")));

    private static Node BuildDomainEventNode(MermaidDomainEvent domainEvent)
        => Node
            .Named(domainEvent.MermaidName)
            .Shaped(NodeShape.Square)
            .Styled(new NodeStyleClass("domainEvents", new NodeStyle("fill:#FFA431")));

    private record MermaidPolicies(IEnumerable<Policy> Policies)
    {
        public MermaidName MermaidNameFor(MermaidCommand command) => new(NameFor(command), FriendlyName, true);
        private static string NameFor(MermaidCommand command) => command.FullName + "Policies";
        private string FriendlyName => Policies.Select(x => x.Description).JoinWith("<br/>");
    }

    private record MermaidCommand(Command Command)
    {
        public MermaidName MermaidName => new(FullName, Command.FriendlyName);
        public string FullName => Command.TypeFullName.Value;
    }

    private record MermaidCollaborator(Collaborator Collaborator)
    {
        public MermaidName MermaidNameFor(MermaidCommand command) => new(NameFor(command), FriendlyName);
        private string NameFor(MermaidCommand command) => command.FullName + MermaidName;
        private string MermaidName => Collaborator.Name.ToPascalCase() + "Collaborator";
        private string FriendlyName => Collaborator.Name;

        public NodeStyleClass GetNodeStyle()
        {
            return Collaborator.Type switch {
                CollaboratorType.Front => new NodeStyleClass("frontCollaborators", new NodeStyle("fill:#FFE5FF")),
                CollaboratorType.BoundedContext => new NodeStyleClass("boundedContextCollaborators", new NodeStyle("fill:#FF5C5C")),
                _ => throw new InvalidOperationException("Unknown collaborator: enable to choose node style")
            };
        }
    }

    private record MermaidDomainEvent(DomainEvent DomainEvent)
    {
        public MermaidName MermaidName => new(DomainEvent.TypeFullName.Value, DomainEvent.FriendlyName);
    }
}