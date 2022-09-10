using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Application.Markdown;

public interface IMarkdownGenerator
{
    Task<string> Render(BoundedContextCanvas extraction, ICanvasSettings canvasSettings);
}