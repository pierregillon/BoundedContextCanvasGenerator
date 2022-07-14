using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public class DefaultGeneratorConfiguration : IGeneratorConfiguration
{
    public TypeDefinitionPredicates CommandsConfiguration => new(new []{ new ImplementsInterfaceMatching(".*ICommand$") });
    public TypeDefinitionPredicates DomainEventsConfiguration => new(new[] { new ImplementsInterfaceMatching(".*IDomainEvent$") });
}