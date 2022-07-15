using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record TypeDefinitionPredicates(IReadOnlyCollection<ITypeDefinitionPredicate> Values)
{
    public bool IsEnabled => Values.Any();
    public bool AllMatching(TypeDefinition type) => Values.All(x => x.IsMatching(type));

    public static TypeDefinitionPredicates Empty() => new(Array.Empty<ITypeDefinitionPredicate>());
    public static TypeDefinitionPredicates From(IEnumerable<ITypeDefinitionPredicate> values) => new(values.ToArray());
    public static TypeDefinitionPredicates From(params ITypeDefinitionPredicate[] values) => new(values);
}