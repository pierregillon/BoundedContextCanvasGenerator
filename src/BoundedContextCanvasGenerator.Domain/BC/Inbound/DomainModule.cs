namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record DomainModule(string Name, IEnumerable<DomainFlow> Flows);