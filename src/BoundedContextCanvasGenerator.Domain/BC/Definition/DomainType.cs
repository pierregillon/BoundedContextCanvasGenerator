namespace BoundedContextCanvasGenerator.Domain.BC.Definition;

public enum DomainType
{
    Unknown,
    CoreDomain
}

public static class DomainTypeExtensions
{
    public static DomainType ToDomainType(this string? value)
    {
        if (value is null) {
            return DomainType.Unknown;
        }

        return value switch {
            "core" => DomainType.CoreDomain,
            _ => throw new InvalidOperationException($"Unknown domain type {value}")
        };
    }
}