using System.Text.RegularExpressions;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

public record NamedLike(string Pattern) : ITypeDefinitionPredicate
{
    private readonly Regex _regex = new(Pattern, RegexOptions.Compiled);

    public bool IsMatching(TypeDefinition type)
        => _regex.IsMatch(type.FullName.Value);

    public virtual bool Equals(NamedLike? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Pattern == other.Pattern;
    }

    public override int GetHashCode() => Pattern.GetHashCode();
}