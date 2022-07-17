namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeFullName(string Value)
{
    public override string ToString() => Value;

    public bool Contains(TypeFullName name) => Value.Contains(name.Value);
    public string Name => Value.Split('.').Last();
    public string Namespace
    {
        get {
            var segments = Value.Split('.');
            return segments
                .Take(segments.Length - 1)
                .JoinWith(".");
        }
    }
}