using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class MarkdownBoundedContextCanvasGenerator
{
    private readonly ITypeDefinitionRepository _typeDefinitionRepository;
    private readonly ICanvasSettingsRepository _canvasSettingsRepository;

    public MarkdownBoundedContextCanvasGenerator(
        ITypeDefinitionRepository typeDefinitionRepository,
        ICanvasSettingsRepository canvasSettingsRepository)
    {
        _typeDefinitionRepository = typeDefinitionRepository;
        _canvasSettingsRepository = canvasSettingsRepository;
    }

    public async Task<string> Generate(SolutionPath solutionPath, CanvasSettingsPath canvasSettingsPath)
    {
        var configuration = await _canvasSettingsRepository.Get(canvasSettingsPath);

        var extractor = new TypeDefinitionExtractor(_typeDefinitionRepository, configuration);

        var extraction = await extractor.Extract(solutionPath);

        return await new MarkdownGenerator().Generate(extraction);
    }
}