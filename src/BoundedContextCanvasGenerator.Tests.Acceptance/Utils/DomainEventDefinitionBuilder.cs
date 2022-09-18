using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class DomainEventDefinitionBuilder
{
    private TypeDefinitionPredicates _predicate = TypeDefinitionPredicates.Empty;
    private HandlerDefinition _handlerDefinition = HandlerDefinition.Empty;

    public DomainEventDefinitionBuilder WithPredicates(TypeDefinitionPredicates predicate)
    {
        _predicate = predicate;
        return this;
    }

    public DomainEventDefinitionBuilder HandledBy(HandlerDefinition handlerDefinition)
    {
        _handlerDefinition = handlerDefinition;
        return this;
    }

    private DomainEventDefinition Build() => new(_predicate, _handlerDefinition);

    public static implicit operator DomainEventDefinition(DomainEventDefinitionBuilder builder) => builder.Build();
}