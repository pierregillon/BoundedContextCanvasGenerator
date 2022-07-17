namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record CanvasSettingsPath(string Value)
{
    private const string DEFAULT_SETTINGS_FILE_NAME = "bounded_context_canvas_settings.yaml";
    public bool IsUndefined => string.IsNullOrEmpty(Value);

    public static CanvasSettingsPath FromPath(string filePath) => new(filePath);

    public static CanvasSettingsPath FromSolutionPath(string solutionFilePath)
    {
        var parentDirectory = Path.GetDirectoryName(solutionFilePath);
        return parentDirectory is null 
            ? FromPath(DEFAULT_SETTINGS_FILE_NAME) 
            : FromPath(Path.Combine(parentDirectory, DEFAULT_SETTINGS_FILE_NAME));
    }
}