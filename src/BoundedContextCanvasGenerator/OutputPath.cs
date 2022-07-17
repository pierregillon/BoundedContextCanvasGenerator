namespace BoundedContextCanvasGenerator;

public record OutputPath(string Value)
{
    private const string DEFAULT_OUTPUT_FILE_NAME = "bounded_context_canvas.md";

    public static OutputPath FromPath(string path) => new(path);

    public static OutputPath FromSolutionPath(string solutionFilePath)
    {
        var parentDirectory = Path.GetDirectoryName(solutionFilePath);
        return parentDirectory is null 
            ? FromPath(DEFAULT_OUTPUT_FILE_NAME) 
            : FromPath(Path.Combine(parentDirectory, DEFAULT_OUTPUT_FILE_NAME));
    }
}