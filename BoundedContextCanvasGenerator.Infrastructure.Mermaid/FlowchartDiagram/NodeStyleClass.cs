namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record NodeStyleClass : IMermaidGeneratable
{
    public string Name { get; init; }
    public NodeStyle Style { get; init; }

    public NodeStyleClass(string name, NodeStyle style)
    {
        this.Name = new MermaidName(name).Id.Value;
        this.Style = style;
    }

    public string ToMermaid(int indentation) => $"{Mermaid.Indent(indentation)}classDef {this.Name} {this.Style.Css};";
}