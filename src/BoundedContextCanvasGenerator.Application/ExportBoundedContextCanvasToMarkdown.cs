using BoundedContextCanvasGenerator.Application.Markdown;
using BoundedContextCanvasGenerator.Domain.BC;

namespace BoundedContextCanvasGenerator.Application;

public class ExportBoundedContextCanvasToMarkdown
{
    private readonly IMarkdownGenerator _markdownGenerator;

    public ExportBoundedContextCanvasToMarkdown(IMarkdownGenerator markdownGenerator) => _markdownGenerator = markdownGenerator;

    public async Task<string> Export(BoundedContextCanvas boundedContextCanvas) => await _markdownGenerator.Render(boundedContextCanvas);
}