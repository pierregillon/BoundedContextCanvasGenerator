namespace BoundedContextCanvasGenerator.Domain.Configuration;

public enum BusinessModel
{
    Unknown,
    RevenueGenerator
}

public static class BusinessModelExtensions
{
    public static BusinessModel ToBusinessModel(this string? value)
    {
        if (value is null)
        {
            return BusinessModel.Unknown;
        }

        return value switch
        {
            "revenue_generator" => BusinessModel.RevenueGenerator,
            _ => throw new InvalidOperationException($"Unknown domain type {value}")
        };
    }
}