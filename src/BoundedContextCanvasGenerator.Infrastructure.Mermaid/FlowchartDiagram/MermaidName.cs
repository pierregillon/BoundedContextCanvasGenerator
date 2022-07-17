using System.Text.RegularExpressions;
using System.Web;

namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record MermaidName
{
    public MermaidId Id { get; }
    public string Label { get; }
    public string EscapedLabel => $@"""{Regex.Replace(HttpUtility.HtmlEncode(this.Label), @"&(#\d+;)", "$1")}""";

    public bool LabelIsId => this.Id.Value == this.Label;

    public static MermaidName HiddenName(string unsafeId) => new(unsafeId, " ");

    public MermaidName(string unsafeId, string label)
    {
        this.Label = label;

        this.Id = new MermaidId(unsafeId);
    }

    public MermaidName(string unsafeId)
    {
        this.Label = unsafeId;

        this.Id = new MermaidId(unsafeId);
    }

    public static implicit operator MermaidName(string value) => new(value);
}