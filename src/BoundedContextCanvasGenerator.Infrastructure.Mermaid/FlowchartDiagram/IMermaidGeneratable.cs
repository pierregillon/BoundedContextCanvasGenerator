namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public interface IMermaidGeneratable
{
    string ToMermaid(int indentation = 1);
}