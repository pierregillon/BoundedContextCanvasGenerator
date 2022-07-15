using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public class TypeDefinitionExtractor : ITypeDefinitionExtractor
{
    private readonly ITypeDefinitionRepository _repository;

    public TypeDefinitionExtractor(ITypeDefinitionRepository repository) => _repository = repository;

    public async Task<TypeDefinitionExtraction> Extract(SolutionPath solutionPath, ICanvasSettings settings)
    {
        var types = _repository.GetAll(solutionPath);

        var commands = new List<TypeDefinition>();
        var domainEvents = new List<TypeDefinition>();
        var aggregates = new List<TypeDefinition>();

        await foreach (var typeDefinition in types)
        {
            if (settings.Commands.IsEnabled && settings.Commands.AllMatching(typeDefinition))
            {
                commands.Add(typeDefinition);
            }

            if (settings.DomainEvents.IsEnabled && settings.DomainEvents.AllMatching(typeDefinition))
            {
                domainEvents.Add(typeDefinition);
            }
            
            if (settings.UbiquitousLanguage.IsEnabled && settings.UbiquitousLanguage.AllMatching(typeDefinition))
            {
                aggregates.Add(typeDefinition);
            }
        }

        return new TypeDefinitionExtraction(
            new ExtractedElements(settings.Commands.IsEnabled, commands), 
            new ExtractedElements(settings.DomainEvents.IsEnabled, domainEvents),
            new ExtractedElements(settings.UbiquitousLanguage.IsEnabled, aggregates)
        );
    }
}