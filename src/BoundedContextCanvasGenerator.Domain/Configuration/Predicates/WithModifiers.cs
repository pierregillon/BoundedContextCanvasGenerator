using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

public record WithModifiers(TypeModifiers TypeModifiers): ITypeDefinitionPredicate
{
    public bool IsMatching(TypeDefinition type) => type.Modifiers.HasFlag(TypeModifiers);

    public static WithModifiers From(TypeModifiers modifiers) => new(modifiers);
}