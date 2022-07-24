using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public record MethodDefinition(MethodInfo Method, IEnumerable<TypeFullName> InstanciatedTypes)
{
    public virtual bool Equals(MethodDefinition? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Method.Equals(other.Method) && InstanciatedTypes.SequenceEqual(other.InstanciatedTypes);
    }

    public override int GetHashCode() => HashCode.Combine(Method, InstanciatedTypes);
}