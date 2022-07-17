using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public static class SpecialSpan
{
    public static MdRawMarkdownSpan NewLine => new("<br/>");
}