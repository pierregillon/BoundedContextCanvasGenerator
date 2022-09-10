using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public class SourceCodeTypeDefinitionExtractor : ITypeDefinitionExtractor
{
    private readonly ITypeDefinitionRepository _repository;

    public SourceCodeTypeDefinitionExtractor(ITypeDefinitionRepository repository) => _repository = repository;

    public async Task<TypeDefinitionExtract> Extract(SolutionPath solutionPath, ICanvasSettings settings)
    {
        var types = _repository.GetAll(solutionPath);

        var commands = new List<TypeDefinition>();
        var domainEvents = new List<TypeDefinition>();
        var aggregates = new List<TypeDefinition>();

        await foreach (var typeDefinition in types) {
            if (settings.InboundCommunicationSettings.CommandPredicates.IsEnabled && settings.InboundCommunicationSettings.CommandPredicates.AllMatching(typeDefinition)) {
                commands.Add(typeDefinition);
            }

            if (settings.DomainEvents.IsEnabled && settings.DomainEvents.AllMatching(typeDefinition)) {
                domainEvents.Add(typeDefinition);
            }

            if (settings.UbiquitousLanguage.IsEnabled && settings.UbiquitousLanguage.AllMatching(typeDefinition)) {
                aggregates.Add(typeDefinition);
            }
        }

        return new TypeDefinitionExtract(
            new ExtractedElements(settings.InboundCommunicationSettings.CommandPredicates.IsEnabled, commands),
            new ExtractedElements(settings.DomainEvents.IsEnabled, domainEvents),
            new ExtractedElements(settings.UbiquitousLanguage.IsEnabled, aggregates)
        );
    }
}