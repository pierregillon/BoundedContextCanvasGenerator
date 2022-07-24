using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public record PolicyDefinition(MethodAttributeMatch Matcher)
{
    public bool Match(MethodInfo method) => method.Attributes.Any(x => Matcher.Match(x));
}