namespace BoundedContextCanvasGenerator.Domain.Types;

public record MethodInfo(MethodName Name, IReadOnlyCollection<MethodAttribute> Attributes)
{
    public virtual bool Equals(MethodInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name.Equals(other.Name) && Attributes.SequenceEqual(other.Attributes);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Attributes);
}