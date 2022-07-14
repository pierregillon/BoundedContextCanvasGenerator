using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class TypeDefinitionExtractor
{
    private readonly ITypeDefinitionRepository _repository;
    private readonly IGeneratorConfiguration _configuration;

    public TypeDefinitionExtractor(ITypeDefinitionRepository repository, IGeneratorConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<TypeDefinitionExtraction> Extract(SolutionPath solutionPath)
    {
        var types = _repository.GetAll(solutionPath);

        var commands = new List<TypeDefinition>();
        var domainEvents = new List<TypeDefinition>();

        await foreach (var typeDefinition in types)
        {
            if (_configuration.CommandsConfiguration.IsEnabled && _configuration.CommandsConfiguration.AllMatching(typeDefinition))
            {
                commands.Add(typeDefinition);
            }

            if (_configuration.DomainEventsConfiguration.IsEnabled && _configuration.DomainEventsConfiguration.AllMatching(typeDefinition))
            {
                domainEvents.Add(typeDefinition);
            }
        }

        return new TypeDefinitionExtraction(
            new ExtractedElements(_configuration.CommandsConfiguration.IsEnabled, commands), 
            new ExtractedElements(_configuration.DomainEventsConfiguration.IsEnabled, domainEvents)
        );
    }
}

public record TypeDefinitionExtraction(
    ExtractedElements Commands,
    ExtractedElements DomainEvents
);

public record ExtractedElements(bool IsEnabled, IReadOnlyCollection<TypeDefinition> Values);