using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public class DefaultCanvasSettings : ICanvasSettings
{
    public TypeDefinitionPredicates Commands => new(new []{ new ImplementsInterfaceMatching(".*ICommand$") });
    public TypeDefinitionPredicates DomainEvents => new(new[] { new ImplementsInterfaceMatching(".*IDomainEvent$") });
}