using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public class DefaultCanvasSettings : ICanvasSettings
{
    public CanvasName Name => CanvasName.Default;
    public CanvasDefinition Definition => CanvasDefinition.Empty;
    public InboundCommunication InboundCommunication =>
        new(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*ICommand$")),
            Enumerable.Empty<CollaboratorDefinition>(),
            Enumerable.Empty<PolicyDefinition>()
        );
    public TypeDefinitionPredicates DomainEvents => new(new[] { new ImplementsInterfaceMatching(".*IDomainEvent$") });
    public TypeDefinitionPredicates UbiquitousLanguage => TypeDefinitionPredicates.Empty;
}