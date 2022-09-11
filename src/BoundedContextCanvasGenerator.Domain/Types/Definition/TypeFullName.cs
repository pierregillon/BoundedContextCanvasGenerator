using System.Diagnostics;

namespace BoundedContextCanvasGenerator.Domain.Types.Definition;

[DebuggerDisplay("{Value}")]
public record TypeFullName(string Value)
{
    public Namespace Namespace { get; } = Namespace.FromResourcePath(Value);

    public override string ToString() => Value;

    public string Name => Value.Split('.').Last();
}