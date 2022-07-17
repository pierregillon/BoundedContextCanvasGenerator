namespace BoundedContextCanvasGenerator.Domain;

public static class FunctionalProgrammingExtensions
{
    public static TResult Pipe<T, TResult>(this T element, Func<T, TResult> projection) => projection(element);
}