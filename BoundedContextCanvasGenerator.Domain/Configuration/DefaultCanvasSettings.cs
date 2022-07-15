using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public class DefaultCanvasSettings : ICanvasSettings
{
    public CanvasName Name => CanvasName.Default;
    public CanvasDefinition Definition => CanvasDefinition.Empty;
    public TypeDefinitionPredicates Commands => new(new []{ new ImplementsInterfaceMatching(".*ICommand$") });
    public TypeDefinitionPredicates DomainEvents => new(new[] { new ImplementsInterfaceMatching(".*IDomainEvent$") });
    public UbiquitousLanguageDefinition UbiquitousLanguage => UbiquitousLanguageDefinition.Empty();
}