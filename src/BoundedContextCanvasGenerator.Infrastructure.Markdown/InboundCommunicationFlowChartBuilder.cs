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

    private static IEnumerable<MdBlock> GenerateModuleBlocks(IReadOnlyCollection<DomainModule> modules)
    {
        if (modules.Count == 1) {
            return new[] { GenerateModuleBlock(modules.Single()) };
        }
        return modules
            .SelectMany(x => new[] {
                new MdHeading(3, x.Name),
                new MdParagraph(new MdRawMarkdownSpan("---")),
                GenerateModuleBlock(x)
            });
    }

    private static MdBlock GenerateModuleBlock(DomainModule domainModule)
    {
        var flowChart = Flowchart.Start(Orientation.LeftToRight);

        var allNodes = domainModule.Flows.SelectMany(GenerateNodes);

        flowChart = allNodes.Aggregate(flowChart, Merge);

        return new MermaidBlock(flowChart);
    }

    private static IEnumerable<IMermaidGeneratable> GenerateNodes(DomainFlow domainFlow)
        => new NodeBuilder(domainFlow).BuildAll();

    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }
}