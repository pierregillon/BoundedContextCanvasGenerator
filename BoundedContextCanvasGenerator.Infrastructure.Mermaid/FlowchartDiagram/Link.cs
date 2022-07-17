namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record Link : IMermaidGeneratable
{
    public IMermaidLinkable Source { get; init; }
    public IMermaidLinkable Target { get; init; }
    private LinkOptions Options { get; init; }
    private string? Text { get; init; }
    private string? Style { get; init; }
    private bool HasNoHeads => this.Options.Head == LinkHead.None || this.Options.Direction == LinkDirection.None;
    private bool HasTwoHeads => this.Options.Direction == LinkDirection.Dual;
    private char? StartChar => !this.HasTwoHeads
        ? null
        : this.Options.Head switch
        {
            LinkHead.Cross => 'x',
            LinkHead.Circle => 'o',
            _ => '<',
        };
    private char? EndChar => this.HasNoHeads
        ? null
        : this.Options.Head switch
        {
            LinkHead.Cross => 'x',
            LinkHead.Circle => 'o',
            _ => '>',
        };

    public Link(IMermaidLinkable source, IMermaidLinkable target)
    {
        this.Source = source;
        this.Target = target;
        this.Options = LinkOptions.Default;
    }

    public Link WithText(string text) => this with { Text = text };

    public Link Styled(string style) => this with { Style = style };

    public Link WithOptions(LinkOptions options)
    {
        if (options.Head == LinkHead.None || options.Direction == LinkDirection.None)
        {
            options = options with
            {
                Head = LinkHead.None,
                Direction = LinkDirection.None
            };
        }

        return this with
        {
            Options = options
        };
    }

    public string ToMermaid(int indentation) => $"{Mermaid.Indent(indentation)}{this.Source.Id}{this.GenerateArrow()}{this.Target.Id}";

    public string? LinkStyleToMermaid(int indentation, int linkIndex) => this.Style == null ? null : $"{Mermaid.Indent(indentation)}linkStyle {linkIndex} {this.Style}";

    private string GenerateArrow()
    {
        string body;
        if (this.Options.LineType == LinkLineType.Dotted)
        {
            body = $"-{new string('.', this.Options.MinimumLength)}-";
        }
        else
        {
            var mainLength = this.Options.MinimumLength + (this.HasNoHeads ? 2 : 1);
            var mainChar = this.Options.LineType is LinkLineType.Thick ? '=' : '-';
            body = new string(mainChar, mainLength);
        }

        return " " + this.StartChar + body + this.EndChar + (this.Text == null ? "" : $"|{this.Text}|") + " ";
    }

    public static LinkBuilder From(IMermaidLinkable source)
    {
        return new(source);
    }

    public record LinkBuilder(IMermaidLinkable Source)
    {
        public Link To(IMermaidLinkable target)
        {
            return new(this.Source, target);
        }
    }
}

public record LinkOptions
{
    public static LinkOptions Default { get; } = new();
    public LinkDirection Direction { get; init; }
    public LinkHead Head { get; init; }
    public LinkLineType LineType { get; init; }
    public int MinimumLength { get; init; }

    private LinkOptions()
    {
        this.Direction = LinkDirection.Single;
        this.Head = LinkHead.Arrow;
        this.LineType = LinkLineType.Straight;
        this.MinimumLength = 1;
    }

    public LinkOptions WithDirection(LinkDirection direction) => this with { Direction = direction };

    public LinkOptions WithHead(LinkHead head) => this with { Head = head };

    public LinkOptions WithLineType(LinkLineType lineType) => this with { LineType = lineType };

    public LinkOptions WithMinimumLength(int minimumLength) => this with { MinimumLength = minimumLength };
}

public enum LinkDirection
{
    None,
    Single,
    Dual,
}

public enum LinkHead
{
    None,
    Arrow,
    Circle,
    Cross,
}

public enum LinkLineType
{
    Straight,
    Thick,
    Dotted,
}