using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class StaticGeneratorConfiguration : IGeneratorConfiguration
{
    public StaticGeneratorConfiguration(ConfigurationDto dto) => CommandDefinitions = dto.Commands.Build();

    public IEnumerable<ITypeDefinitionPredicate> CommandDefinitions { get; }
}