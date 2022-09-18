using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class IntegrationEventDefinitionBuilder
{
    private TypeDefinitionPredicates _predicates = TypeDefinitionPredicates.Empty;

    public IntegrationEventDefinitionBuilder WithPredicates(TypeDefinitionPredicates predicates)
    {
        this._predicates = predicates;
        return this;
    }

    private IntegrationEventDefinition Build() => new(this._predicates);

    public static implicit operator IntegrationEventDefinition(IntegrationEventDefinitionBuilder builder) => builder.Build();
}