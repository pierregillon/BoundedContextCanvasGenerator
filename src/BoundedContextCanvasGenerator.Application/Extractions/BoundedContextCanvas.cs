using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public record BoundedContextCanvas(
    CanvasName Name,
    CanvasDefinition Definition,
    ExtractedElements Commands,
    ExtractedElements DomainEvents, 
    ExtractedElements Aggregates,
    InboundCommunication InboundCommunication
);