namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record ConfigurationPath(string Value)
{
    public bool IsUndefined => string.IsNullOrEmpty(Value);
}