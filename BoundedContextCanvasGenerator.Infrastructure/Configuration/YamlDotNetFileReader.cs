using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;
using YamlDotNet.Serialization.NamingConventions;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class YamlDotNetFileReader : IYamlFileReader
{
    public async Task<IGeneratorConfiguration> Read(string filePath)
    {
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var fileContent = await File.ReadAllTextAsync(filePath);

        var dto = deserializer.Deserialize<ConfigurationDto>(fileContent);

        return new StaticGeneratorConfiguration(dto);
    }
}