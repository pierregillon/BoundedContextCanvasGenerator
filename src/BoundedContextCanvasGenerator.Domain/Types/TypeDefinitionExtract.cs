namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeDefinitionExtract(
    ExtractedElements Commands,
    ExtractedElements DomainEvents,
    ExtractedElements Aggregates,
    IReadOnlyCollection<LinkedTypeDefinition> CommandHandlers
);