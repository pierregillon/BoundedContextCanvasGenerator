using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public static class ISelectableExtensions
{
    public static TypeDefinitionPredicates BuildPredicates(this ISelectable selectable)
    {
        var analyser = new PredicateAnalyser();

        return selectable
            .Selector?
            .Pipe(analyser.Analyse)
            .Pipe(TypeDefinitionPredicates.From) ?? TypeDefinitionPredicates.Empty;
    }
}