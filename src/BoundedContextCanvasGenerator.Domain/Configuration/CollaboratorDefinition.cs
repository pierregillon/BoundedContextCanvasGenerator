using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public record CollaboratorDefinition(string Name, TypeDefinitionPredicates Predicates)
{
    public bool Match(TypeDefinition type) => Predicates.AllMatching(type);
}