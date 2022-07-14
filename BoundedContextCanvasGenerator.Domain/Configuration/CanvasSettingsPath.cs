namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record CanvasSettingsPath(string Value)
{
    public bool IsUndefined => string.IsNullOrEmpty(Value);
}