using System.Text.RegularExpressions;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

public record ImplementsInterfaceMatching(string Pattern) : ITypeDefinitionPredicate
{
    private readonly Regex _regex = new(Pattern, RegexOptions.Compiled);

    public bool IsMatching(TypeDefinition type) 
        => type.ImplementedInterfaces.Any(x => _regex.IsMatch(x.Value));

    public virtual bool Equals(ImplementsInterfaceMatching? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Pattern == other.Pattern;
    }

    public override int GetHashCode() => Pattern.GetHashCode();
}