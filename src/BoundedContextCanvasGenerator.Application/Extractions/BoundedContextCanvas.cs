namespace BoundedContextCanvasGenerator.Application.Extractions;

public record BoundedContextCanvas(
    ExtractedElements Commands,
    ExtractedElements DomainEvents, 
    ExtractedElements Aggregates
);