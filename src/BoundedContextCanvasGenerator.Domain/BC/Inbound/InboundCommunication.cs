namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record InboundCommunication(IEnumerable<DomainModule> Modules)
{
    public static InboundCommunication Empty => new(Enumerable.Empty<DomainModule>());
    public bool IsNotEmpty => this != Empty;

    public virtual bool Equals(InboundCommunication? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Modules.SequenceEqual(other.Modules);
    }

    public override int GetHashCode() => Modules.GetHashCode();
}