using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Types;

public class TypeDefinitionFilter
{
    public TypeDefinitionExtract Filter(IReadOnlyCollection<TypeDefinition> types, ICanvasSettings settings)
    {
        var commands = new List<TypeDefinition>();
        var domainEvents = new List<TypeDefinition>();
        var aggregates = new List<TypeDefinition>();

        foreach (var typeDefinition in types) {
            if (settings.InboundCommunicationSettings.CommandPredicates.IsEnabled && settings.InboundCommunicationSettings.CommandPredicates.AllMatching(typeDefinition)) {
                commands.Add(typeDefinition);
            }

            if (settings.InboundCommunicationSettings.DomainEventDefinitions.IsEnabled && settings.InboundCommunicationSettings.DomainEventDefinitions.AllMatching(typeDefinition)) {
                domainEvents.Add(typeDefinition);
            }

            if (settings.UbiquitousLanguage.IsEnabled && settings.UbiquitousLanguage.AllMatching(typeDefinition)) {
                aggregates.Add(typeDefinition);
            }
        }

        return new TypeDefinitionExtract(
            new ExtractedElements(settings.InboundCommunicationSettings.CommandPredicates.IsEnabled, commands),
            new ExtractedElements(settings.InboundCommunicationSettings.DomainEventDefinitions.IsEnabled, domainEvents),
            new ExtractedElements(settings.UbiquitousLanguage.IsEnabled, aggregates)
        );
    }
}