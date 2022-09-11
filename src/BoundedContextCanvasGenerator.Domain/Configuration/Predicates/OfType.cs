using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

public record OfType(TypeKind TypeKind) : ITypeDefinitionPredicate
{
    public bool IsMatching(TypeDefinition type) => TypeKind == type.Kind;
}