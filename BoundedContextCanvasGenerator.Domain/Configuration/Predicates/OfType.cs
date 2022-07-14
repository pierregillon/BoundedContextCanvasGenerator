using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

public class OfType : ITypeDefinitionPredicate
{
    private readonly string _typeName;

    public OfType(string typeName) => _typeName = typeName;

    public bool IsMatching(TypeDefinition type)
    {
        return _typeName switch {
            "class" => type.Type == TypeType.Class,
            "interface" => type.Type == TypeType.Interface,
            _ => throw new InvalidOperationException($"Unknown type name {_typeName}")
        };
    }
}