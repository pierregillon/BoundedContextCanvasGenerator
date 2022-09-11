using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class InboundCommunicationFlowChartBuilder
{
    private readonly InboundCommunication _inboundCommunication;

    public InboundCommunicationFlowChartBuilder(InboundCommunication inboundCommunication)
    {
        if (inboundCommunication == null || inboundCommunication == InboundCommunication.Empty) {
            throw new ArgumentNullException(nameof(inboundCommunication));
        }

        _inboundCommunication = inboundCommunication;
    }

    public MdContainerBlock Build() => new(GenerateModuleBlocks(_inboundCommunication.Modules.ToArray()));

    private IEnumerable<MdBlock> GenerateModuleBlocks(IReadOnlyCollection<DomainModule> modules)
    {
        if (modules.Count() == 1) {
            return new[] { GenerateModuleBlock(modules.Single()) };
        }
        return modules
            .SelectMany(x => new[] {
                new MdHeading(3, x.Name),
                new MdParagraph(new MdRawMarkdownSpan("---")),
                GenerateModuleBlock(x)
            });
    }

    private MdBlock GenerateModuleBlock(DomainModule domainModule)
    {
        var flowChart = Flowchart.Start(Orientation.LeftToRight);

        var allNodes = domainModule.Flows.SelectMany(GenerateNodes);

        flowChart = allNodes.Aggregate(flowChart, Merge);

        return new MermaidBlock(flowChart);
    }

    private IEnumerable<IMermaidGeneratable> GenerateNodes(DomainFlow domainFlow)
    {
        var mermaidCommand = new MermaidCommand(domainFlow.Command);

        var commandNode = BuildCommandNode(mermaidCommand);

        yield return commandNode;

        foreach (var collaboratorNode in BuildCollaboratorNodes(domainFlow, mermaidCommand)) {
            yield return collaboratorNode;
            yield return Link.From(collaboratorNode).To(commandNode);
        }

        if (!domainFlow.Policies.Any()) yield break;

        var policiesNode = BuildPolicyNode(new MermaidPolicies(domainFlow.Policies), mermaidCommand);

        yield return policiesNode;
        yield return Link
            .From(commandNode)
            .To(policiesNode)
            .WithOptions(LinkOptions.Default.WithHead(LinkHead.None));
    }

    private static IEnumerable<Node> BuildCollaboratorNodes(DomainFlow domainFlow, MermaidCommand mermaidCommand)
    {
        return domainFlow.Collaborators
            .Select(x => new MermaidCollaborator(x))
            .Select(x => BuildCollaboratorNode(x, mermaidCommand))
            .ToArray();
    }

    private static Node BuildCommandNode(MermaidCommand command)
        => Node.Named(command.MermaidName);

    private static Node BuildCollaboratorNode(MermaidCollaborator mermaidCollaborator, MermaidCommand command)
        => Node
            .Named(mermaidCollaborator.MermaidNameFor(command))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyleClass("collaborators", new NodeStyle("fill:#FFE5FF")));

    private static Node BuildPolicyNode(MermaidPolicies policies, MermaidCommand command)
        => Node
            .Named(policies.MermaidNameFor(command))
            .Shaped(NodeShape.Parallelogram)
            .Styled(new NodeStyleClass("policies", new NodeStyle("fill:#FFFFAD, font-style:italic")));

    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }

    private record MermaidPolicies(IEnumerable<Policy> Policies)
    {
        public MermaidName MermaidNameFor(MermaidCommand command) => new(NameFor(command), FriendlyName, true);
        private static string NameFor(MermaidCommand command) => command.FullName + "Policies";
        private string FriendlyName => Policies.Select(x => x.Description).JoinWith("<br/>");
    }

    private record MermaidCommand(Command Command)
    {
        public MermaidName MermaidName => new(this.FullName, this.Command.FriendlyName);
        public string FullName => Command.TypeFullName.Value;
    }

    private record MermaidCollaborator(Collaborator Collaborator)
    {
        public MermaidName MermaidNameFor(MermaidCommand command) => new(NameFor(command), FriendlyName);
        private string NameFor(MermaidCommand command) => command.FullName + MermaidName;
        private string MermaidName => Collaborator.Name.ToPascalCase() + "Collaborator";
        private string FriendlyName => Collaborator.Name;
    }
}