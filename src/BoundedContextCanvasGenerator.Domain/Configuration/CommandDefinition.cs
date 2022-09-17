using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record CommandDefinition(TypeDefinitionPredicates Predicates, HandlerDefinition Handler)
{
    public static CommandDefinition Empty => new(TypeDefinitionPredicates.Empty, HandlerDefinition.Empty);
}

public record HandlerDefinition(TypeDefinitionPredicates Predicates, TypeDefinitionLink Link)
{
    public static HandlerDefinition Empty => new(TypeDefinitionPredicates.Empty, TypeDefinitionLink.Empty);

}