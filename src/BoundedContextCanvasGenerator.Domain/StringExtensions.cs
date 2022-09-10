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
    public static string ToPascalCase(this string value) => new(PascalCaseCharacters(value).ToArray());

    private static IEnumerable<char> AddSpaceCharBetweenWords(string value)
    {
        for (var i = 0; i < value.Length; i++) {
            var current = value[i];
            if (i == 0) {
                yield return current;
            }
            else {
                var previous = value[i - 1];
                if (char.IsLower(previous) && (char.IsUpper(current) || char.IsDigit(current))) {
                    yield return ' ';
                }
                if (current == '_') {
                    yield return ' ';
                }
                else {
                    yield return char.ToLower(current);
                }
            }
        }
    }

    private static IEnumerable<char> PascalCaseCharacters(string value)
    {
        for (var index = 0; index < value.Length; index++) {
            var character = value[index];
            if (index == 0) {
                yield return char.ToUpper(character);
            }
            else {
                if (character == ' ') continue;
                var previousCharacter = value[index - 1];
                if (previousCharacter == ' ') {
                    yield return char.ToUpper(character);
                }
                else {
                    yield return character;
                }
            }
        }
    }
}