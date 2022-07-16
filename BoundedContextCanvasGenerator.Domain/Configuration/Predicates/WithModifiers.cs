using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

public class WithModifiers : ITypeDefinitionPredicate
{
    private readonly TypeModifiers _typeModifiers;

    public WithModifiers(TypeModifiers typeModifiers) => _typeModifiers = typeModifiers;

    public bool IsMatching(TypeDefinition type) => type.Modifiers.HasFlag(_typeModifiers);

    public static WithModifiers From(TypeModifiers modifiers) => new(modifiers);
}