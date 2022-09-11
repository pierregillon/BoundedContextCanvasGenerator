using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class GenerateBoundedContextCanvasFromSolutionPath
{
    private readonly ICanvasSettingsRepository _canvasSettingsRepository;
    private readonly ITypeDefinitionRepository _typeDefinitionRepository;
    private readonly TypeDefinitionFilter _typeDefinitionExtractor;
    private readonly BoundedContextCanvasAnalyser _boundedContextCanvasAnalyser;

    public GenerateBoundedContextCanvasFromSolutionPath(
        ICanvasSettingsRepository canvasSettingsRepository,
        ITypeDefinitionRepository typeDefinitionRepository,
        TypeDefinitionFilter typeDefinitionExtractor,
        BoundedContextCanvasAnalyser boundedContextCanvasAnalyser)
    {
        _canvasSettingsRepository = canvasSettingsRepository;
        _typeDefinitionRepository = typeDefinitionRepository;
        _typeDefinitionExtractor = typeDefinitionExtractor;
        _boundedContextCanvasAnalyser = boundedContextCanvasAnalyser;
    }

    public async Task<BoundedContextCanvas> Generate(SolutionPath solutionPath, CanvasSettingsPath canvasSettingsPath)
    {
        var canvasSettings = await _canvasSettingsRepository.Get(canvasSettingsPath);

        var typeDefinitions = await _typeDefinitionRepository.GetAll(solutionPath);

        var typeDefinitionExtract = _typeDefinitionExtractor.Filter(typeDefinitions, canvasSettings);

        return await _boundedContextCanvasAnalyser.Analyse(typeDefinitionExtract, canvasSettings);
    }
}