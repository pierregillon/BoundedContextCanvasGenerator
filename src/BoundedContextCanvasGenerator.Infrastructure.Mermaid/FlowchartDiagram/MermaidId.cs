using System.Text.RegularExpressions;

namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record MermaidId
{
    public string Value { get; }

    public MermaidId(string value)
    {
        this.Value = Regex.Replace(value, @"[\W_]", "");
    }

    public override string ToString()
    {
        return this.Value;
    }

    public static implicit operator MermaidId(string value) => new(value);
}