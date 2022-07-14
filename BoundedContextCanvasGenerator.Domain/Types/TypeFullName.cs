namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeFullName(string Value)
{
    public override string ToString() => Value;

    public bool Contains(TypeFullName name) => Value.Contains(name.Value);
}