using System;
using System.Collections.Generic;
using System.Linq;

namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record Node : IMermaidGeneratable, IMermaidLinkable
{
    private MermaidName Name { get; init; }
    private NodeShape Shape { get; init; }
    private NodeStyle? Style { get; init; }
    public NodeStyleClass? StyleClass { get; init; }
    private string? Url { get; init; }

    public MermaidId Id => this.Name.Id;
    public string Label => this.Name.Label;

    private Node(MermaidName name)
    {
        this.Name = name;
        this.Shape = NodeShape.Default;
    }

    public static Node Named(MermaidName name) => new(name);
    public static Node Named(string name) => new(name);

    public Node Labeled(string label) => this with
    {
        Name = new MermaidName(this.Name.Id.Value, label)
    };

    public Node Shaped(NodeShape shape) => this with
    {
        Shape = shape
    };

    public Node Styled(NodeStyle style) => this with
    {
        Style = style
    };

    public Node Styled(NodeStyleClass styleClass) => this with
    {
        StyleClass = styleClass
    };

    public Node ClickableToUrl(string url) => this with
    {
        Url = url,
    };

    public string ToMermaid(int indentation)
    {
        var lines = new List<string>();

        var hasImplicitLabel = this.Shape == NodeShape.Default && this.Name.LabelIsId;
        var label = hasImplicitLabel ? "" : this.Shape.FormatLabel(this.Name.EscapedLabel);
        var declaration = this.Id + label;
        lines.Add(declaration);

        if (this.Url != null)
        {
            lines.Add(@$"click {this.Id} href ""{this.Url}""");
        }

        if (this.StyleClass != null)
        {
            lines.Add($"class {this.Id} {this.StyleClass.Name};");
        }

        if (this.Style != null)
        {
            lines.Add($"style {this.Id} {this.Style.Css}");
        }

        var indent = Mermaid.Indent(indentation);
        return string.Join(
            Environment.NewLine,
            lines.Select(line => indent + line));
    }
}