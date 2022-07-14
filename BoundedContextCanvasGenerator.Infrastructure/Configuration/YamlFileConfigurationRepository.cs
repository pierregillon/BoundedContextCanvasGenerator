using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class YamlFileConfigurationRepository : IConfigurationRepository
{
    private readonly string _configurationFilePath;

    public YamlFileConfigurationRepository(string configurationFilePath) => _configurationFilePath = configurationFilePath;

    public async Task<IGeneratorConfiguration> Get()
    {
        var fileContent = await File.ReadAllTextAsync(_configurationFilePath);

        var dto = new YamlDotNetConfigurationDeserializer().Deserialize(fileContent);

        return new StaticGeneratorConfiguration(dto);
    }
}