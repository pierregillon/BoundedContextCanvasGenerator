using BoundedContextCanvasGenerator;

public class StaticGeneratorConfiguration : IGeneratorConfiguration
{
    public StaticGeneratorConfiguration(ConfigurationDto dto) => CommandDefinitions = dto.Commands.Build();

    public IEnumerable<ITypeDefinitionPredicate> CommandDefinitions { get; }
}