using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Application.Markdown;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class MarkdownBoundedContextCanvasGenerator
{
    private readonly ICanvasSettingsRepository _canvasSettingsRepository;
    private readonly IBoundedContextCanvasAnalyser _boundedContextCanvasAnalyser;
    private readonly IMarkdownGenerator _markdownGenerator;

    public MarkdownBoundedContextCanvasGenerator(
        ICanvasSettingsRepository canvasSettingsRepository,
        IBoundedContextCanvasAnalyser boundedContextCanvasAnalyser,
        IMarkdownGenerator markdownGenerator)
    {
        _canvasSettingsRepository = canvasSettingsRepository;
        _boundedContextCanvasAnalyser = boundedContextCanvasAnalyser;
        _markdownGenerator = markdownGenerator;
    }

    public async Task<string> Generate(SolutionPath solutionPath, CanvasSettingsPath canvasSettingsPath)
    {
        var canvasSettings = await _canvasSettingsRepository.Get(canvasSettingsPath);

        var boundedContextCanvas = await _boundedContextCanvasAnalyser.Analyse(solutionPath, canvasSettings);

        return await _markdownGenerator.Render(boundedContextCanvas, canvasSettings);
    }
}
