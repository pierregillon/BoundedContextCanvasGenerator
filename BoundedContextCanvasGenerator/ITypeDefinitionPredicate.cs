using LivingDocumentation.Domain;

namespace BoundedContextCanvasGenerator;

public interface ITypeDefinitionPredicate
{
    bool IsMatching(TypeDefinition type);
}