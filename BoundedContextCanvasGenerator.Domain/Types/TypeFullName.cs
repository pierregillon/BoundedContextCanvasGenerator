namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeFullName(string Value)
{
    public Namespace Namespace { get; } = Namespace.FromResourcePath(Value);

    public override string ToString() => Value;

    public string Name => Value.Split('.').Last();


}