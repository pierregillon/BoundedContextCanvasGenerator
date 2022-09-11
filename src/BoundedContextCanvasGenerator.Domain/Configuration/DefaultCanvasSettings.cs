using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public class DefaultCanvasSettings : ICanvasSettings
{
    public CanvasName Name => CanvasName.Default;
    public CanvasDefinition Definition => CanvasDefinition.Empty;
    public InboundCommunicationSettings InboundCommunicationSettings =>
        new(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*ICommand$")),
            Enumerable.Empty<CollaboratorDefinition>(),
            Enumerable.Empty<PolicyDefinition>(),
            TypeDefinitionPredicates.Empty
        );
    public TypeDefinitionPredicates DomainEvents => new(new[] { new ImplementsInterfaceMatching(".*IDomainEvent$") });
    public TypeDefinitionPredicates UbiquitousLanguage => TypeDefinitionPredicates.Empty;
}