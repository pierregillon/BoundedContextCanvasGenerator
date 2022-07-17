namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record NodeShape
{
    public static NodeShape Default => Square;
    public static NodeShape Square { get; } = new("[", "]");
    public static NodeShape RoundEdges { get; } = new("(", ")");
    public static NodeShape Stadium { get; } = new("([", "])");
    public static NodeShape Subroutine { get; } = new("[[", "]]");
    public static NodeShape Cylindrical { get; } = new("[(", ")]");
    public static NodeShape Circle { get; } = new("((", "))");
    public static NodeShape Asymmetric { get; } = new(">", "]");
    public static NodeShape Rhombus { get; } = new("{", "}");
    public static NodeShape Hexagon { get; } = new("{{", "}}");
    public static NodeShape Parallelogram { get; } = new("[/", "/]");
    public static NodeShape ParallelogramAlt { get; } = new("[\\", "\\]");
    public static NodeShape Trapezoid { get; } = new("[/", "\\]");
    public static NodeShape TrapezoidAlt { get; } = new("[\\", "/]");

    private readonly string startLabel;
    private readonly string endLabel;
    private NodeShape(string startLabel, string endLabel)
    {
        this.startLabel = startLabel;
        this.endLabel = endLabel;
    }

    public string FormatLabel(string nameSafeLabel)
    {
        return $"{this.startLabel}{nameSafeLabel}{this.endLabel}";
    }
}