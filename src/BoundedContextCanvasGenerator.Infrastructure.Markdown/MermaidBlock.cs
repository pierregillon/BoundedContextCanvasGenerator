using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public record MermaidBlock(Flowchart Flowchart)
{
    private const string MERMAID_LANGUAGE = "mermaid";
    
    public MdCodeBlock ToCodeBlock() => new(Flowchart.ToMermaid(), MERMAID_LANGUAGE);

    public static implicit operator MdCodeBlock(MermaidBlock block) => block.ToCodeBlock();
}