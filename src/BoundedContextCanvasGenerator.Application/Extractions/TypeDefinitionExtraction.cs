namespace BoundedContextCanvasGenerator.Application.Extractions;

public record TypeDefinitionExtraction(
    ExtractedElements Commands,
    ExtractedElements DomainEvents, 
    ExtractedElements Aggregates
);