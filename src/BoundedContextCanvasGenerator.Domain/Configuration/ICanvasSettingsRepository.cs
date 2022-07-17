namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface ICanvasSettingsRepository
{
    Task<ICanvasSettings> Get(CanvasSettingsPath canvasSettingsPath);
}