using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Types;

public record LinkedTypeDefinition(TypeDefinition TypeDefinition, TypeDefinitionLink HandlerLink)
{
    public bool Match(TypeDefinition source) => HandlerLink.AreLinked(source, TypeDefinition);
}