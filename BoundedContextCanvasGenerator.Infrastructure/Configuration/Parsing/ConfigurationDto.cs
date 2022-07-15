using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class ConfigurationDto
{
    public string? Name { get; set; }
    public CanvasDefinitionDto? Definition { get; set; }
    public CommandConfigurationDto? Commands { get; set; }
    public DomainEventConfigurationDto? DomainEvents { get; set; }
    public UbiquitousLanguageDefinitionDto? UbiquitousLanguage { get; set; }
}

public class UbiquitousLanguageDefinitionDto
{
    public string? Type { get; set; }
    public ImplementingConfigurationDto? Implementing { get; set; }

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
    }
}