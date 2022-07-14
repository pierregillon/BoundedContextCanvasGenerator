namespace BoundedContextCanvasGenerator.Application;

public static class StringExtensions
{
    public static string JoinLines(this IEnumerable<string> elements) 
        => string.Join(Environment.NewLine, elements);
}