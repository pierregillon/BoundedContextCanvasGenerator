using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class YamlFileConfigurationRepository : IConfigurationRepository
{
    public async Task<IGeneratorConfiguration> Get(ConfigurationPath configurationPath)
    {
        if (configurationPath.IsUndefined)
        {
            return new DefaultGeneratorConfiguration();
        }

        var fileContent = await File.ReadAllTextAsync(configurationPath.Value);

        var dto = new YamlDotNetConfigurationDeserializer().Deserialize(fileContent);

        return new StaticGeneratorConfiguration(dto);
    }
}