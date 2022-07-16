using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class UbiquitousLanguageDefinitionDto
{
    public string? Type { get; set; }
    public ImplementingConfigurationDto? Implementing { get; set; }
    public string[]? Modifiers { get; set; }

    public UbiquitousLanguageDefinition Build() => UbiquitousLanguageDefinition.From(GetAll());

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
        if (Modifiers is not null) {
            yield return Modifiers
                .Select(x => x.ToTypeModifiers())
                .Aggregate()
                .Pipe(WithModifiers.From);
        }
    }
}