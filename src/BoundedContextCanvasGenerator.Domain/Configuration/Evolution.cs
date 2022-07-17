namespace BoundedContextCanvasGenerator.Domain.Configuration;

public enum Evolution
{
    Unknown,
    Commodity
}

public static class EvolutionExtensions
{
    public static Evolution ToEvolution(this string? value)
    {
        if (value is null)
        {
            return Evolution.Unknown;
        }

        return value switch
        {
            "commodity" => Evolution.Commodity,
            _ => throw new InvalidOperationException($"Unknown domain type {value}")
        };
    }
}