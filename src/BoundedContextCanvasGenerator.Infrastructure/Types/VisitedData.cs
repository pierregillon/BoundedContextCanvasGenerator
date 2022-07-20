using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class VisitedData
{
    private readonly List<TypeDefinition> _typeDefinitions = new();
    private readonly Dictionary<TypeFullName, List<MethodDefinition>> _methods = new();

    public IReadOnlyCollection<TypeDefinition> TypeDefinitions => _typeDefinitions;
    public IDictionary<TypeFullName, List<MethodDefinition>> Methods => _methods;

    public void AddTypeDefinition(TypeDefinition typeDefinition) => _typeDefinitions.Add(typeDefinition);

    public void AddMethod(TypeFullName typeDefinition, MethodDefinition method)
    {
        if (this.Methods.TryGetValue(typeDefinition, out var methods)) {
            methods.Add(method);
        }
        else {
            this.Methods.Add(typeDefinition, new List<MethodDefinition>{ method });
        }
    }
}

public record MethodDefinition(string Name, IEnumerable<TypeFullName> InstanciatedTypes)
{
    public virtual bool Equals(MethodDefinition? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && InstanciatedTypes.SequenceEqual(other.InstanciatedTypes);
    }

    public override int GetHashCode() => HashCode.Combine(Name, InstanciatedTypes);
}