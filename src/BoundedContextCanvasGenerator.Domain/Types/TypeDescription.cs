namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeDescription(string Value)
{
    public static TypeDescription Empty => From(string.Empty);

    public static TypeDescription From(string value) => new(value);
}