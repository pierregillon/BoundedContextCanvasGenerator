using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class InboundCommunicationFlowChartBuilder
{
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

        return new MermaidBlock(flowChart);
    }

    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }

}