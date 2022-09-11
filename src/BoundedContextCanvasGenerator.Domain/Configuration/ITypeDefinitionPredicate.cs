using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface ITypeDefinitionPredicate
{
    bool IsMatching(TypeDefinition type);
}