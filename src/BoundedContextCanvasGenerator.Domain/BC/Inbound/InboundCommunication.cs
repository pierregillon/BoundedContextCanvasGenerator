namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record InboundCommunication(IEnumerable<DomainModule> Modules)
{
    public static InboundCommunication Empty => new(Enumerable.Empty<DomainModule>());
    public bool IsNotEmpty => this != Empty;
}