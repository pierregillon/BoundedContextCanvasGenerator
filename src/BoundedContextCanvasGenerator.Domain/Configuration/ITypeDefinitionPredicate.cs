using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface ITypeDefinitionPredicate
{
    bool IsMatching(TypeDefinition type);
}