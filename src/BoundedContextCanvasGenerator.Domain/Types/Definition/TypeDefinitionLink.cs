using System.Text.RegularExpressions;

namespace BoundedContextCanvasGenerator.Domain.Types.Definition;

public record TypeDefinitionLink
{
    private static readonly Regex StructureRegex = new("(?<type>.*?) *-> *(?<expr>.*)", RegexOptions.Compiled);
    private readonly string _type;
    private readonly string _expression;

    private TypeDefinitionLink(string value)
    {
        var match = StructureRegex.Match(value);
        _type = match.Groups["type"].Value;
        _expression = match.Groups["expr"].Value;
    }

    public static TypeDefinitionLink From(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        }
        if (!StructureRegex.IsMatch(value)) {
            throw new ArgumentException("Bad structure on type definition link", nameof(value));
        }
        return new TypeDefinitionLink(value);
    }

    public static TypeDefinitionLink Empty => new(string.Empty);

    public bool AreLinked(TypeDefinition source, TypeDefinition destination)
    {
        var regexValue = _expression.Replace(_type, Regex.Escape(source.FullName.Value));
        var regex = new Regex(regexValue, RegexOptions.Compiled);
        return destination.ImplementedInterfaces.Any(x => regex.IsMatch(x.Value));
    }
}