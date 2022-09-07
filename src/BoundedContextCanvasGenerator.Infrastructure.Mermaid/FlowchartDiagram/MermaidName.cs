using System.Text.RegularExpressions;
using System.Web;

namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record MermaidName
{
    public MermaidId Id { get; }
    public string Label { get; }
    public string EscapedLabel { get; }

    public bool LabelIsId => this.Id.Value == this.Label;

    public static MermaidName HiddenName(string unsafeId) => new(unsafeId, " ");

    public MermaidName(string unsafeId, string label, bool doNotEscapeLabel = false)
    {
        this.Label = label;

        this.EscapedLabel = doNotEscapeLabel 
            ? $@"""{this.Label}"""
            : $@"""{Regex.Replace(HttpUtility.HtmlEncode(this.Label), @"&(#\d+;)", "$1")}""";

        this.Id = new MermaidId(unsafeId);
    }

    public MermaidName(string unsafeId)
    {
        this.Label = unsafeId;

        this.Id = new MermaidId(unsafeId);
    }

    public static implicit operator MermaidName(string value) => new(value);
}