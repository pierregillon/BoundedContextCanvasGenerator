using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class DomainEventConfigurationDto
{
    public string? Type { get; set; }
    public ImplementingConfigurationDto? Implementing { get; set; }

    public TypeDefinitionPredicates Build() => TypeDefinitionPredicates.From(GetAll());

    private IEnumerable<ITypeDefinitionPredicate> GetAll()
    {
        if (Type is not null)
        {
            yield return new OfType(Type.ToTypeKind());
        }
        if (Implementing is not null)
        {
            yield return Implementing.Build();
        }
    }
}