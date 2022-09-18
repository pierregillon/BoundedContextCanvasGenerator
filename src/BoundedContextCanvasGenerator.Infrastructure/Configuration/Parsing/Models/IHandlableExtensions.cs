using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public static class IHandlableExtensions
{
    public static HandlerDefinition BuildHandler(this HandlerDefinitionDto? handlerDto)
    {
        if (handlerDto is null) {
            return HandlerDefinition.Empty;
        }
        return new HandlerDefinition(
            handlerDto.BuildPredicates(),
            TypeDefinitionLink.Parse(handlerDto.Link)
        );
    }
}