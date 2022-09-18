using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeDefinitionExtract(ExtractedElements Commands,
    ExtractedElements DomainEvents,
    ExtractedElements Aggregates,
    IReadOnlyCollection<LinkedTypeDefinition> Handlers, 
    IReadOnlyCollection<TypeDefinition> IntegrationEvents);