namespace BoundedContextCanvasGenerator.Domain.Configuration;

public class ConfigurationFactory
{
    private readonly IYamlFileReader _yamlFileReader;

    public ConfigurationFactory(IYamlFileReader yamlFileReader) => _yamlFileReader = yamlFileReader;

    public async Task<IGeneratorConfiguration> Build(string? configurationFilePath)
    {
        if (configurationFilePath is null) {
            return new DefaultGeneratorConfiguration();
        }

        return await _yamlFileReader.Read(configurationFilePath);
    }
}