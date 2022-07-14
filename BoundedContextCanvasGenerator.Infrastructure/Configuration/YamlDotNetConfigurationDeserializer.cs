using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;
using YamlDotNet.Serialization.NamingConventions;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class YamlDotNetConfigurationDeserializer
{
    public ConfigurationDto Deserialize(string plainText)
    {
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<ConfigurationDto>(plainText);
    }
}