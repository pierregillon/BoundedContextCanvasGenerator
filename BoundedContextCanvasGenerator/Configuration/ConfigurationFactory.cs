using BoundedContextCanvasGenerator;
using YamlDotNet.Serialization.NamingConventions;

public class ConfigurationFactory
{
    public async Task<IGeneratorConfiguration> Build(string? configurationFilePath)
    {
        if (configurationFilePath is null) {
            return new DefaultGeneratorConfiguration();
        }

        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var dto = deserializer.Deserialize<ConfigurationDto>(await File.ReadAllTextAsync(configurationFilePath));

        return new StaticGeneratorConfiguration(dto);
    }
}