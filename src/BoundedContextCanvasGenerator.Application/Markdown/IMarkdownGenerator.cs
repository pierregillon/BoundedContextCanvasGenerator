using BoundedContextCanvasGenerator.Domain.BC;

namespace BoundedContextCanvasGenerator.Application.Markdown;

public interface IMarkdownGenerator
{
    Task<string> Render(BoundedContextCanvas boundedContextCanvas);
}