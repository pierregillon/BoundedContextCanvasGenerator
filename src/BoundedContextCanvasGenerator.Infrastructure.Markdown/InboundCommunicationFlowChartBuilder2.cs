using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class InboundCommunicationFlowChartBuilder2
{
    private readonly IEnumerable<CollaboratorDefinition2> _collaboratorDefinitions;
    private readonly IEnumerable<PolicyDefinition> _policyDefinitions;

    public InboundCommunicationFlowChartBuilder2(IEnumerable<CollaboratorDefinition2> collaboratorDefinitions, IEnumerable<PolicyDefinition> policyDefinitions)
    {
        _collaboratorDefinitions = collaboratorDefinitions;
        _policyDefinitions = policyDefinitions;
    }

    public MdContainerBlock Build(IReadOnlyCollection<TypeDefinition> typeDefinitions)
    {
        if (typeDefinitions.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(typeDefinitions));

        var commands = typeDefinitions.Select(x => new Command(x)).ToArray();

        return new MdContainerBlock(GenerateMermaidBlock(commands, false));
    }

    private MdCodeBlock GenerateMermaidBlock(IEnumerable<Command> commands, bool splitIntoLanes)
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
            .ToArray();

        foreach (var collaboratorNode in collaboratorNodes) {
            yield return collaboratorNode;
            yield return Link.From(collaboratorNode).To(node);
        }

        var policies = command.TypeDefinition.Instanciators
            .Where(x => _policyDefinitions.Any(c => c.Match(x.Method)))
            .Select(x => x.Method.Name.Value.ToReadableSentence())
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

    private static Node BuildNode(CollaboratorDefinition2 collaborator, Command command)
        => Node
            .Named(new MermaidName(collaborator.NameFor(command), collaborator.FriendlyName))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyleClass("collaborators", new NodeStyle("fill:#FFE5FF")));    
    
    private static Node BuildNode(Policies policies, Command command)
        => Node
            .Named(new MermaidName(policies.NameFor(command), policies.FriendlyName))
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