using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

public class OfType : ITypeDefinitionPredicate
{
    private readonly TypeKind _typeKind;

    public OfType(TypeKind typeKind) => _typeKind = typeKind;

    public bool IsMatching(TypeDefinition type) => _typeKind == type.Kind;
}