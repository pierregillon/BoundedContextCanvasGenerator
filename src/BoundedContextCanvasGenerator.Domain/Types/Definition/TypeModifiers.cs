namespace BoundedContextCanvasGenerator.Domain.Types.Definition;

[Flags]
public enum TypeModifiers
{
    None = 0,
    Abstract = 1 << 0,
    Concrete = 1 << 1
}

public static class TypeModifiersExtensions
{
    public static TypeModifiers AddFlag(this TypeModifiers current, TypeModifiers toRemove) => current | toRemove;
    public static TypeModifiers RemoveFlag(this TypeModifiers current, TypeModifiers toRemove) => current & ~toRemove;

    public static TypeModifiers ToTypeModifiers(this string value)
    {
        if (!Enum.TryParse(typeof(TypeModifiers), value, true, out var result)) {
            throw new InvalidOperationException($"Unable to parse '{value}' into a {nameof(TypeModifiers).ToReadableSentence()}.");
        }
        return (TypeModifiers)result!;
    }

    public static TypeModifiers Aggregate(this IEnumerable<TypeModifiers> typeModifiers) 
        => typeModifiers.Aggregate(TypeModifiers.None, (x, y) => x | y);
}