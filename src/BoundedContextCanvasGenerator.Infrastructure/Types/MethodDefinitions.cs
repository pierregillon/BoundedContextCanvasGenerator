using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class MethodDefinitions
{
    private readonly Dictionary<TypeFullName, List<MethodDefinition>> _methods;

    public MethodDefinitions() => _methods = new Dictionary<TypeFullName, List<MethodDefinition>>();

    public void AddMethod(TypeFullName typeDefinition, MethodDefinition method)
    {
        if (_methods.TryGetValue(typeDefinition, out var methods))
        {
            methods.Add(method);
        }
        else
        {
            _methods.Add(typeDefinition, new List<MethodDefinition> { method });
        }
    }

    public IEnumerable<(TypeFullName, MethodInfo)> FindInstanciators(TypeDefinition typeDefinition)
    {
        return _methods
            .Where(x => x.Value.Any(m => m.InstanciatedTypes.Contains(typeDefinition.FullName)))
            .SelectMany(x => x.Value.Select(m => (x.Key, m.Method)))
            .ToArray();
    }

    protected bool Equals(MethodDefinitions other)
    {
        if (_methods.Count != other._methods.Count) {
            return false;
        }
        
        if (!_methods.Keys.SequenceEqual(other._methods.Keys)) {
            return false;
        }

        for (int i = 0; i < _methods.Count; i++) {
            var methodValues = _methods.Values.ElementAt(i);
            var otherMethodValues = other._methods.Values.ElementAt(i);

            if (!methodValues.SequenceEqual(otherMethodValues)) {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MethodDefinitions)obj);
    }

    public override int GetHashCode() => _methods.GetHashCode();
}