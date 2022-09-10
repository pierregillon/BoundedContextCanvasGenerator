using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class GenerateBoundedContextCanvasFromSolutionPath
{
    private readonly ICanvasSettingsRepository _canvasSettingsRepository;
    private readonly ITypeDefinitionExtractor _typeDefinitionExtractor;
    private readonly IBoundedContextCanvasAnalyser _boundedContextCanvasAnalyser;

    public GenerateBoundedContextCanvasFromSolutionPath(
        ICanvasSettingsRepository canvasSettingsRepository,
        ITypeDefinitionExtractor typeDefinitionExtractor,
        IBoundedContextCanvasAnalyser boundedContextCanvasAnalyser)
    {
        _canvasSettingsRepository = canvasSettingsRepository;
        _typeDefinitionExtractor = typeDefinitionExtractor;
        _boundedContextCanvasAnalyser = boundedContextCanvasAnalyser;
    }

    public async Task<BoundedContextCanvas> Generate(SolutionPath solutionPath, CanvasSettingsPath canvasSettingsPath)
    {
        var canvasSettings = await _canvasSettingsRepository.Get(canvasSettingsPath);

        var typeDefinitionExtract = await _typeDefinitionExtractor.Extract(solutionPath, canvasSettings);

        return await _boundedContextCanvasAnalyser.Analyse(typeDefinitionExtract);
    }
}