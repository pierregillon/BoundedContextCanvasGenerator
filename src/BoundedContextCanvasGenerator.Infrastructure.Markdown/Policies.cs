using BoundedContextCanvasGenerator.Domain;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public record Policies(IEnumerable<string> Values)
{
    private const string MermaidName = "Policies";
    public string FriendlyName => Values.JoinWith("<br/>");

    public static Policies From(IEnumerable<string> values) => new(values);

    public bool Any() => this.Values.Any();

    public string NameFor(Command command) => command.MermaidName + MermaidName;
}