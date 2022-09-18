using System.Diagnostics;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

[DebuggerDisplay("MethodDefinitions : {_methods.Count}")]
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

    public IEnumerable<(TypeFullName, IEnumerable<MethodInfo>)> FindInstanciators(TypeDefinition typeDefinition)
    {
        return _methods
            .Select(x => new {
                Name = x.Key,
                MatchingMethods = x.Value
                    .Where(method => method.IsInstanciating(typeDefinition.FullName))
                    .Select(definition => definition.Method)
                    .ToArray()
            })
            .Where(x => x.MatchingMethods.Length > 0)
            .Select(x => (x.Name, x.MatchingMethods.AsEnumerable()))
            .ToArray();
    }

    protected bool Equals(MethodDefinitions other)
    {
        if (_methods.Count != other._methods.Count) {
            return false;
        }
        
        if (!_methods.Keys.OrderBy(x => x.Value).SequenceEqual(other._methods.Keys.OrderBy(x => x.Value))) {
            return false;
        }

        for (int i = 0; i < _methods.Count; i++) {
            var key = _methods.Keys.ElementAt(i);
            var methodValues = _methods[key];
            var otherMethodValues = other._methods[key];

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