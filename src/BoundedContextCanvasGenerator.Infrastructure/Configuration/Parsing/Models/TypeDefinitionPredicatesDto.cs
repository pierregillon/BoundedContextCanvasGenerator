using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public class TypeDefinitionPredicatesDto
{
    public string? Selector { get; set; }

    public TypeDefinitionPredicates Build()
    {
        if (Selector is null) {
            throw new InvalidOperationException("Unable to select types : selector is null");
        }
        return TypeDefinitionPredicates.From(new PredicateAnalyser().Analyse(Selector));
    }
}