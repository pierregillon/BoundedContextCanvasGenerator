using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Types;

public class TypeDefinitionFilter
{
    public TypeDefinitionExtract Filter(IReadOnlyCollection<TypeDefinition> types, ICanvasSettings settings)
    {
        var commands = new List<TypeDefinition>();
        var commandHandlers = new List<LinkedTypeDefinition>();
        var domainEvents = new List<TypeDefinition>();
        var aggregates = new List<TypeDefinition>();
        var integrationEvents = new List<TypeDefinition>();

        foreach (var typeDefinition in types) {
            if (settings.InboundCommunicationSettings.CommandDefinition.Predicates.IsEnabled && 
                settings.InboundCommunicationSettings.CommandDefinition.Predicates.AllMatching(typeDefinition)) {
                commands.Add(typeDefinition);
            }
            
            if (settings.InboundCommunicationSettings.CommandDefinition.Handler.Predicates.IsEnabled && 
                settings.InboundCommunicationSettings.CommandDefinition.Handler.Predicates.AllMatching(typeDefinition)) {
                commandHandlers.Add(new LinkedTypeDefinition(typeDefinition, settings.InboundCommunicationSettings.CommandDefinition.Handler.Link));
            }

            if (settings.InboundCommunicationSettings.DomainEventDefinitions.Predicates.IsEnabled && 
                settings.InboundCommunicationSettings.DomainEventDefinitions.Predicates.AllMatching(typeDefinition)) {
                domainEvents.Add(typeDefinition);
            }

            if (settings.InboundCommunicationSettings.DomainEventDefinitions.Handler.Predicates.IsEnabled &&
                settings.InboundCommunicationSettings.DomainEventDefinitions.Handler.Predicates.AllMatching(typeDefinition))
            {
                commandHandlers.Add(new LinkedTypeDefinition(typeDefinition, settings.InboundCommunicationSettings.DomainEventDefinitions.Handler.Link));
            }

            if (settings.InboundCommunicationSettings.IntegrationEventDefinition.Predicates.IsEnabled &&
                settings.InboundCommunicationSettings.IntegrationEventDefinition.Predicates.AllMatching(typeDefinition)) {
                integrationEvents.Add(typeDefinition);
            }

            if (settings.UbiquitousLanguage.IsEnabled && 
                settings.UbiquitousLanguage.AllMatching(typeDefinition)) {
                aggregates.Add(typeDefinition);
            }
        }

        return new TypeDefinitionExtract(
            new ExtractedElements(settings.InboundCommunicationSettings.CommandDefinition.Predicates.IsEnabled, commands),
            new ExtractedElements(settings.InboundCommunicationSettings.DomainEventDefinitions.Predicates.IsEnabled, domainEvents),
            new ExtractedElements(settings.UbiquitousLanguage.IsEnabled, aggregates),
            commandHandlers,
            integrationEvents
        );
    }
}