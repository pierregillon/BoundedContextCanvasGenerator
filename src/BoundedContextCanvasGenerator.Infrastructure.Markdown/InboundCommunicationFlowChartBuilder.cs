using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class InboundCommunicationFlowChartBuilder
{
    private readonly IEnumerable<MermaidCollaboratorDefinition> _collaboratorDefinitions;
    private readonly IEnumerable<PolicyDefinition> _policyDefinitions;

    public InboundCommunicationFlowChartBuilder(IEnumerable<MermaidCollaboratorDefinition> collaboratorDefinitions, IEnumerable<PolicyDefinition> policyDefinitions)
    {
        _collaboratorDefinitions = collaboratorDefinitions;
        _policyDefinitions = policyDefinitions;
    }

    public MdContainerBlock Build(IReadOnlyCollection<TypeDefinition> typeDefinitions)
    {
        if (typeDefinitions.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(typeDefinitions));

        var commands = typeDefinitions.Select(x => new Command(x)).ToArray();

        return new MdContainerBlock(GenerateLanes(commands));
    }

    private IEnumerable<MdBlock> GenerateLanes(IEnumerable<Command> commands)
    {
        var commandByLanes = commands.GroupBy(x => x.Lane).ToArray();
        if (commandByLanes.Length == 1) {
            return new MdBlock[]{ GenerateAllCommands(commandByLanes.Single()) };
        }
        return commandByLanes
            .SelectMany(x => new MdBlock[] {
                new MdHeading(3, x.Key.Name),
                new MdParagraph(new MdRawMarkdownSpan("---")),
                GenerateAllCommands(x)
            });
    }

    private MdCodeBlock GenerateAllCommands(IEnumerable<Command> commands)
    {
        var flowChart = Flowchart.Start(Orientation.LeftToRight);

        var allNodes = commands.SelectMany(GenerateCommandNodes);

        flowChart = allNodes.Aggregate(flowChart, Merge);

        return new MermaidBlock(flowChart);
    }

    private IEnumerable<IMermaidGeneratable> GenerateCommandNodes(Command command)
    {
        var node = BuildNode(command);

        yield return node;

        var collaboratorNodes = command.TypeDefinition.Instanciators
            .Select(x => new {
                Instanciator = x,
                Collaborators = _collaboratorDefinitions.Where(c => c.Match(x.Type))
            })
            .SelectMany(x => x.Collaborators.Select(c => BuildNode(c, command)))
            .Distinct()
            .ToArray();

        foreach (var collaboratorNode in collaboratorNodes) {
            yield return collaboratorNode;
            yield return Link.From(collaboratorNode).To(node);
        }

        var policies = command.TypeDefinition.Instanciators
            .SelectMany(instanciator => instanciator.FilterMethodsMatching(_policyDefinitions))
            .Select(method => method.Name.Value.ToReadableSentence())
            .ToArray()
            .Pipe(Policies.From);

        if (policies.Any()) {
            var policyNode = BuildNode(policies, command);
            yield return policyNode;
            yield return Link.From(node).To(policyNode).WithOptions(LinkOptions.Default.WithHead(LinkHead.None));
        }
    }

    private static Node BuildNode(Command command)
        => Node.Named(new MermaidName(command.MermaidName, command.FriendlyName));

    private static Node BuildNode(MermaidCollaboratorDefinition mermaidCollaborator, Command command)
        => Node
            .Named(new MermaidName(mermaidCollaborator.NameFor(command), mermaidCollaborator.FriendlyName))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyleClass("collaborators", new NodeStyle("fill:#FFE5FF")));    
    
    private static Node BuildNode(Policies policies, Command command)
        => Node
            .Named(new MermaidName(policies.NameFor(command), policies.FriendlyName, true))
            .Shaped(NodeShape.Parallelogram)
            .Styled(new NodeStyleClass("policies", new NodeStyle("fill:#FFFFAD, font-style:italic")));


    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch
        {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }
}

public class InboundCommunicationFlowChartBuilder2
{
    private readonly InboundCommunication _inboundCommunication;

    public InboundCommunicationFlowChartBuilder2(InboundCommunication inboundCommunication)
    {
        if (inboundCommunication == null || inboundCommunication == InboundCommunication.Empty)
        {
            throw new ArgumentNullException(nameof(inboundCommunication));
        }

        _inboundCommunication = inboundCommunication;
    }

    public MdContainerBlock Build() => new(GenerateModuleBlocks(_inboundCommunication.Modules.ToArray()));

    private IEnumerable<MdBlock> GenerateModuleBlocks(IReadOnlyCollection<DomainModule> modules)
    {
        if (modules.Count() == 1) {
            return new []{ GenerateModuleBlock(modules.Single()) };
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
        => Node.Named(new MermaidName(command.MermaidName, command.FriendlyName));

    private static Node BuildCollaboratorNode(MermaidCollaborator mermaidCollaborator, MermaidCommand command)
        => Node
            .Named(new MermaidName(mermaidCollaborator.NameFor(command), mermaidCollaborator.FriendlyName))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyleClass("collaborators", new NodeStyle("fill:#FFE5FF")));

    private static Node BuildPolicyNode(MermaidPolicies policies, MermaidCommand command)
        => Node
            .Named(new MermaidName(policies.NameFor(command), policies.FriendlyName, true))
            .Shaped(NodeShape.Parallelogram)
            .Styled(new NodeStyleClass("policies", new NodeStyle("fill:#FFFFAD, font-style:italic")));

    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch
        {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }

}

internal record MermaidPolicies(IEnumerable<Policy> Policies)
{
    public string FriendlyName => Policies.Select(x => x.Description).JoinWith("<br/>");
    public string NameFor(MermaidCommand command) => command.MermaidName + "Policies";
}

public record MermaidCommand(Domain.BC.Inbound.Command Command)
{
    public string MermaidName => Command.TypeFullName.Value;
    public string FriendlyName => Command.Name;
}

public record MermaidCollaborator(Collaborator Collaborator)
{
    public string MermaidName => Collaborator.Name.ToPascalCase() + "Collaborator";
    public string FriendlyName => Collaborator.Name;

    public string NameFor(MermaidCommand command) => command.MermaidName + this.MermaidName;
}