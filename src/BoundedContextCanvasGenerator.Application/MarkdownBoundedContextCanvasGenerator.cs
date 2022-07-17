using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Application.Markdown;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class MarkdownBoundedContextCanvasGenerator
{
    private readonly ICanvasSettingsRepository _canvasSettingsRepository;
    private readonly ITypeDefinitionExtractor _extractor;
    private readonly IMarkdownGenerator _markdownGenerator;

    public MarkdownBoundedContextCanvasGenerator(
        ICanvasSettingsRepository canvasSettingsRepository,
        ITypeDefinitionExtractor extractor,
        IMarkdownGenerator markdownGenerator)
    {
        _canvasSettingsRepository = canvasSettingsRepository;
        _extractor = extractor;
        _markdownGenerator = markdownGenerator;
    }

    public async Task<string> Generate(SolutionPath solutionPath, CanvasSettingsPath canvasSettingsPath)
    {
        var configuration = await _canvasSettingsRepository.Get(canvasSettingsPath);

        var extraction = await _extractor.Extract(solutionPath, configuration);

        return await _markdownGenerator.Generate(extraction, configuration);
    }
}