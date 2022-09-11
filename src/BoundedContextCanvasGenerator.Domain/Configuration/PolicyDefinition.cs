using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record PolicyDefinition(MethodAttributeMatch Matcher)
{
    public bool Match(MethodInfo method) => method.Attributes.Any(x => Matcher.Match(x));
}