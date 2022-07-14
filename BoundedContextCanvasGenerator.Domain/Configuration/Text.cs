namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record Text(string Value)
{
    public static Text Empty => new(string.Empty);
    public bool IsEmpty => this == Empty;

    public static Text From(string? value) => value is null ? Empty : new Text(value);
}