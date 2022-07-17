using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class TypeDefinitionPredicatesDto
{
    public string? Type { get; set; }
    public ImplementingConfigurationDto? Implementing { get; set; }
    public string[]? Modifiers { get; set; }

    public TypeDefinitionPredicates Build() => TypeDefinitionPredicates.From(GetPredicates());

    private IEnumerable<ITypeDefinitionPredicate> GetPredicates()
    {
        if (Type is not null)
        {
            yield return new OfType(Type.ToTypeKind());
        }
        if (Implementing is not null)
        {
            yield return Implementing.Build();
        }
        if (Modifiers is not null)
        {
            yield return Modifiers
                .Select(x => x.ToTypeModifiers())
                .Aggregate()
                .Pipe(WithModifiers.From);
        }
    }
}