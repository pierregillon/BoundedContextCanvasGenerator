using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class InboundCommunicationFlowChartBuilder
{
    private const string MERMAID_LANGUAGE = "mermaid";

    private readonly IReadOnlyCollection<Command> _commands;

    public static InboundCommunicationFlowChartBuilder From(IReadOnlyCollection<TypeDefinition> types) => new(types.Select(x => new Command(x)).ToArray());

    public InboundCommunicationFlowChartBuilder(IReadOnlyCollection<Command> commands)
    {
        if (commands.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(commands));

        _commands = commands;
    }

    public MdContainerBlock Build(bool splitIntoLanes) => 
        !splitIntoLanes 
            ? new MdContainerBlock(GenerateAllCommands()) 
            : new MdContainerBlock(GenerateLanes());

    private MdCodeBlock GenerateAllCommands() => GenerateMermaidBlock(_commands, false);

    private IEnumerable<MdBlock> GenerateLanes()
    {
        return _commands
            .GroupBy(x => x.Lane)
            .SelectMany(x => new MdBlock[] {
                new MdHeading(3, x.Key.Name),
                new MdParagraph(new MdRawMarkdownSpan("---")),
                GenerateMermaidBlock(x, true)
            });
    }

    private static MdCodeBlock GenerateMermaidBlock(IEnumerable<Command> commands, bool splitIntoLanes)
    {
        var flowChart = Flowchart.Start(Orientation.LeftToRight);

        var root = Node
            .Named(new MermaidName("Collaborators", "WebApp"))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyle("fill:#f9f,stroke:#333,stroke-width:2px"));

        flowChart = flowChart.WithNode(root);
        flowChart = new Tree(root, splitIntoLanes)
            .GenerateNodes(commands)
            .Aggregate(flowChart, Merge);

        return MermaidBlock(flowChart);
    }

    private static MdCodeBlock MermaidBlock(Flowchart flowChart) => new(flowChart.ToMermaid(), MERMAID_LANGUAGE);

    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }

}

public record Tree(Node Root, bool splitIntoLanes)
{
    private readonly IDictionary<Namespace, Node> _alreadyCreatedNamespaceNodes = new Dictionary<Namespace, Node>();

    public IEnumerable<IMermaidGeneratable> GenerateNodes(IEnumerable<Command> commands) => commands.SelectMany(GenerateNodes);

    private IEnumerable<IMermaidGeneratable> GenerateNodes(Command command)
    {
        var namespaceNodes = GenerateNamespaceNodes(command).ToArray();
        var commandNodes = GenerateCommandNodes(command).ToArray();
        return namespaceNodes.Concat(commandNodes);
    }

    private IEnumerable<IMermaidGeneratable> GenerateNamespaceNodes(Command command)
    {
        Node? previousNamespaceNode = null;
        foreach (var subNamespace in command.GetSubNamespaces(splitIntoLanes))
        {
            if (this.TryCreateNamespaceNode(subNamespace, out var namespaceNode))
            {
                yield return namespaceNode;
                yield return Link.From(previousNamespaceNode ?? Root).To(namespaceNode);
            }
            previousNamespaceNode = namespaceNode;
        }
    }

    private IEnumerable<IMermaidGeneratable> GenerateCommandNodes(Command command)
    {
        var node = BuildNode(command);
        yield return node;
        if (_alreadyCreatedNamespaceNodes.TryGetValue(command.ParentNamespace, out var parentNamespaceNode)) {
            yield return Link.From(parentNamespaceNode).To(node);
        }
        else {
            yield return Link.From(Root).To(node);
        }
    }

    private bool TryCreateNamespaceNode(Namespace @namespace, out Node result)
    {
        if (_alreadyCreatedNamespaceNodes.TryGetValue(@namespace, out var node))
        {
            result = node;
            return false;
        }
        var namespaceNode = BuildNode(@namespace);
        _alreadyCreatedNamespaceNodes.Add(@namespace, namespaceNode);
        result = namespaceNode;
        return true;
    }

    private static Node BuildNode(Namespace @namespace) 
        => Node.Named(new MermaidName(@namespace.Path, @namespace.Name));

    private static Node BuildNode(Command command)
        => Node.Named(new MermaidName(command.FullName, command.FriendlyName));
}

public record Command(TypeDefinition TypeDefinition)
{
    public Namespace ParentNamespace { get; } = TypeDefinition.FullName.Namespace;
    public string FullName => TypeDefinition.FullName.Value;
    public string FriendlyName => TypeDefinition.FullName.Name.TrimWord("Command").ToReadableSentence();
    public Namespace Lane => ParentNamespace.TrimStart(TypeDefinition.AssemblyDefinition.Namespace);

    public IEnumerable<Namespace> GetSubNamespaces(bool splitIntoLanes)
    {
        return ParentNamespace
            .GetSubNamespaces()
            .Where(@namespace => !TypeDefinition.AssemblyDefinition.Namespace.StartWith(@namespace))
            .Where(@namespace => !splitIntoLanes || !@namespace.EndWith(Lane))
            ;
    }
}