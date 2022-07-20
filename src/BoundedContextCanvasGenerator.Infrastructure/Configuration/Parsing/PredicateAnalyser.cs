using System.Text.RegularExpressions;
using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class PredicateAnalyser
{
    private static readonly string KindRegex = $"(?<Kind>{Enum.GetValues<TypeKind>().Select(x => x.ToString().ToLower()).JoinWith('|')})";
    private const string ModifierRegex = "((?<Modifier>\\w*) )?";
    private const string ImplementingRegex = "( implementing '(?<Implementing>\\S*)')?";

    private readonly Regex _regex = new($"{ModifierRegex}{KindRegex}{ImplementingRegex}");

    public IEnumerable<ITypeDefinitionPredicate> Analyse(string selector)
    {
        var match = _regex.Match(selector);
        if (match is null) {
            throw new InvalidOperationException("Invalid format");
        }
        if (TryGetGroup(match, "Modifier", out var modifiers)) {
            yield return new WithModifiers(Enum.Parse<TypeModifiers>(modifiers, true));
        }

        if (TryGetGroup(match, "Kind", out var kind)) {
            yield return new OfType(Enum.Parse<TypeKind>(kind, true));
        }

        if (TryGetGroup(match, "Implementing", out var pattern)) {
            yield return new ImplementsInterfaceMatching(pattern);
        }
    }

    private static bool TryGetGroup(Match match, string groupName, out string result)
    {
        if (!string.IsNullOrWhiteSpace(match.Groups[groupName].Value)) {
            result = match.Groups[groupName].Value;
            return true;
        }
        result = string.Empty;
        return false;
    }
}