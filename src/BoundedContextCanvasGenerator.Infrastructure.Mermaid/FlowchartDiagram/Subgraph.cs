using System;
using System.Collections.Generic;
using System.Linq;

namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record Subgraph : IMermaidGeneratable, IMermaidLinkable
{
    private readonly MermaidName name;
    public Flowchart Content { get; }
    public MermaidId Id => this.name.Id;

    public NodeStyleClass? StyleClass { get; private init; }
    public NodeStyle? Style { get; private init; }

    public Subgraph(MermaidName name, Flowchart content)
    {
        this.name = name;
        this.Content = content;
    }

    public string ToMermaid(int indentation)
    {
        var indent = Mermaid.Indent(indentation);
        var label = this.name.LabelIsId ? "" : $"[{this.name.EscapedLabel}]";
        var lines = new List<string>();
        lines.Add(
            $@"subgraph {this.name.Id}{label}
{this.Content.ToMermaid(indentation + 1)}");
        lines.Add("end");
        if (this.Style != null)
        {
            lines.Add($"style {this.Id} {this.Style.Css}");
        }
        if (this.StyleClass != null)
        {
            lines.Add($"class {this.Id} {this.StyleClass.Name};");
        }
        return string.Join(
            Environment.NewLine,
            lines.Select(line => indent + line));
    }

    public static SubgraphBuilder Named(MermaidName name)
    {
        return new SubgraphBuilder(name);
    }

    public Subgraph Styled(NodeStyleClass styleClass) =>
        this with
        {
            StyleClass = styleClass
        };

    public Subgraph Styled(NodeStyle style) =>
        this with
        {
            Style = style
        };

    public record SubgraphBuilder(MermaidName Name)
    {
        public Subgraph WithContent(Flowchart content)
        {
            return new Subgraph(Name, content);
        }
    }
}