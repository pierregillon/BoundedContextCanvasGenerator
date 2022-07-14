using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class ImplementingConfigurationDto
{
    public string? Pattern { get; set; }

    public ITypeDefinitionPredicate Build()
    {
        if (Pattern is null) {
            throw new InvalidOperationException("Pattern must be defined");
        }

        return new ImplementsInterfaceMatching(Pattern);
    }
}