using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class StaticGeneratorConfiguration : IGeneratorConfiguration
{
    public StaticGeneratorConfiguration(ConfigurationDto dto)
    {
        CommandsConfiguration = dto?.Commands?.Build() ?? TypeDefinitionPredicates.Empty();
    }

    public TypeDefinitionPredicates CommandsConfiguration { get; }
}