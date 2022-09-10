using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;

namespace BoundedContextCanvasGenerator.Domain.BC;

public record BoundedContextCanvas(
    CanvasName Name,
    CanvasDefinition Definition,
    UbiquitousLanguage UbiquitousLanguage,
    InboundCommunication InboundCommunication
);