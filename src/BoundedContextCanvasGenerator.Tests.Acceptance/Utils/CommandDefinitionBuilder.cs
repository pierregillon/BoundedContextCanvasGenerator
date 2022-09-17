using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class CommandDefinitionBuilder
{
    private TypeDefinitionPredicates _commandPredicates = TypeDefinitionPredicates.Empty;
    private HandlerDefinition _handlerPredicates = HandlerDefinition.Empty;

    public CommandDefinitionBuilder WithPredicates(TypeDefinitionPredicates predicates)
    {
        this._commandPredicates = predicates;
        return this;
    }

    public CommandDefinitionBuilder HandledBy(HandlerDefinition handlerDefinition)
    {
        this._handlerPredicates = handlerDefinition;
        return this;
    }

    private CommandDefinition Build() => new(this._commandPredicates, this._handlerPredicates);

    public static implicit operator CommandDefinition(CommandDefinitionBuilder builder) => builder.Build();
}