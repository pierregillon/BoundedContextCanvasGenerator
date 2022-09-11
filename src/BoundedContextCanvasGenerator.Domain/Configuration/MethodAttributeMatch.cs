using System.Text.RegularExpressions;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record MethodAttributeMatch(string Pattern)
{
    private readonly Regex _regex = new(Pattern, RegexOptions.Compiled);

    public bool Match(MethodAttribute attribute)
        => _regex.IsMatch(attribute.Value);

    public virtual bool Equals(ImplementsInterfaceMatching? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Pattern == other.Pattern;
    }

    public override int GetHashCode() => Pattern.GetHashCode();
}