using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class MarkdownBoundedContextCanvasGenerator
{
    private readonly ITypeDefinitionRepository _typeDefinitionRepository;
    private readonly IConfigurationRepository _configurationRepository;

    public MarkdownBoundedContextCanvasGenerator(
        ITypeDefinitionRepository typeDefinitionRepository,
        IConfigurationRepository configurationRepository)
    {
        _typeDefinitionRepository = typeDefinitionRepository;
        _configurationRepository = configurationRepository;
    }

    public async Task<string> Generate(SolutionPath solutionPath, ConfigurationPath configurationPath)
    {
        var configuration = await _configurationRepository.Get(configurationPath);

        var extractor = new TypeDefinitionExtractor(_typeDefinitionRepository, configuration);

        var extraction = await extractor.Extract(solutionPath);

        return await new MarkdownGenerator().Generate(extraction);
    }
}