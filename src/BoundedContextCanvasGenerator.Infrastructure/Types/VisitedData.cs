using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class VisitedData
{
    private readonly List<TypeDefinition> _typeDefinitions;
    private readonly Dictionary<TypeFullName, List<MethodDefinition>> _methods;

    public IReadOnlyCollection<TypeDefinition> TypeDefinitions => _typeDefinitions;
    public IDictionary<TypeFullName, List<MethodDefinition>> Methods => _methods;

    public VisitedData()
    {
        _typeDefinitions = new List<TypeDefinition>();
        _methods = new Dictionary<TypeFullName, List<MethodDefinition>>();
    }
    public VisitedData(IEnumerable<TypeDefinition> typeDefinitions, IEnumerable<KeyValuePair<TypeFullName, List<MethodDefinition>>> methods)
    {
        _typeDefinitions = new List<TypeDefinition>(typeDefinitions);
        _methods = new Dictionary<TypeFullName, List<MethodDefinition>>(methods);
    }

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

    public static VisitedData operator +(VisitedData first, VisitedData second)
    {
        return new VisitedData(
            first.TypeDefinitions.Concat(second.TypeDefinitions),
            first.Methods.Concat(second.Methods)
        );
    }
}

public class VisitedData2
{
    private readonly Dictionary<TypeFullName, List<MethodDefinition>> _methods;

    public IDictionary<TypeFullName, List<MethodDefinition>> Methods => _methods;

    public VisitedData2()
    {
        _methods = new Dictionary<TypeFullName, List<MethodDefinition>>();
    }
    public VisitedData2(IEnumerable<TypeDefinition> typeDefinitions, IEnumerable<KeyValuePair<TypeFullName, List<MethodDefinition>>> methods)
    {
        _methods = new Dictionary<TypeFullName, List<MethodDefinition>>(methods);
    }

    public void AddMethod(TypeFullName typeDefinition, MethodDefinition method)
    {
        if (this.Methods.TryGetValue(typeDefinition, out var methods))
        {
            methods.Add(method);
        }
        else
        {
            this.Methods.Add(typeDefinition, new List<MethodDefinition> { method });
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