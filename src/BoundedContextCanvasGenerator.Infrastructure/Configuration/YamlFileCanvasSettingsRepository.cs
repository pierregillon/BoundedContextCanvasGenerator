using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class YamlFileCanvasSettingsRepository : ICanvasSettingsRepository
{
    public async Task<ICanvasSettings> Get(CanvasSettingsPath canvasSettingsPath)
    {
        if (canvasSettingsPath.IsUndefined) {
            return new DefaultCanvasSettings();
        }

        var fileContent = await File.ReadAllTextAsync(canvasSettingsPath.Value);

        var dto = new YamlDotNetConfigurationDeserializer().Deserialize(fileContent);

        return new StaticCanvasSettings(dto);
    }
}