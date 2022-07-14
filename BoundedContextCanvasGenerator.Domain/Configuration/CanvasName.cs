namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record CanvasName(string Value)
{
    public static CanvasName Default => new("Bounded context canvas");

    public static CanvasName From(string? value) => string.IsNullOrWhiteSpace(value) ? CanvasName.Default : new CanvasName(value);
}