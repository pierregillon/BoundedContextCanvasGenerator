using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface ICanvasSettings
{
    public CanvasName Name { get; }
    public CanvasDefinition Definition { get; }
    public TypeDefinitionPredicates Commands { get; }
    public TypeDefinitionPredicates DomainEvents { get; }
    public UbiquitousLanguageDefinition UbiquitousLanguage { get; }
}

public record UbiquitousLanguageDefinition(IReadOnlyCollection<ITypeDefinitionPredicate> Values)
{
    public bool IsEnabled => Values.Any();
    public bool AllMatching(TypeDefinition type) => Values.All(x => x.IsMatching(type));

    public static UbiquitousLanguageDefinition Empty() => new(Array.Empty<ITypeDefinitionPredicate>());
    public static UbiquitousLanguageDefinition From(IEnumerable<ITypeDefinitionPredicate> values) => new(values.ToArray());
    public static UbiquitousLanguageDefinition From(params ITypeDefinitionPredicate[] values) => new(values);

}