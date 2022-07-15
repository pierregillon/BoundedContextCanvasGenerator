using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Application.Markdown;

public interface IMarkdownGenerator
{
    Task<string> Generate(TypeDefinitionExtraction extraction, ICanvasSettings canvasSettings);
}