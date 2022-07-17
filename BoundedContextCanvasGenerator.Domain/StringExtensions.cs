
namespace BoundedContextCanvasGenerator.Domain;

public static class StringExtensions
{
    public static string JoinLines(this IEnumerable<string> elements)
        => elements.JoinWith(Environment.NewLine);

    public static string JoinWith(this IEnumerable<string> elements, string separator)
        => string.Join(separator, elements);

    public static string JoinWith(this IEnumerable<string> elements, char separator)
        => string.Join(separator, elements);

    public static string SurroundWith(this string value, string left, string right)
        => $"{left}{value}{right}";

    public static string TrimWord(this string value, string word) => value.EndsWith(word) ? value[..^word.Length] : value;
    public static string ToReadableSentence(this string value) => new(AddSpaceCharBetweenWords(value).ToArray());
    
    private static IEnumerable<char> AddSpaceCharBetweenWords(string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            char current = value[i];
            if (i == 0)
            {
                yield return current;
            }
            else
            {
                char previous = value[i - 1];
                if (char.IsLower(previous) && (char.IsUpper(current) || char.IsDigit(current)))
                {
                    yield return ' ';
                }
                yield return char.ToLower(current);
            }
        }
    }
}