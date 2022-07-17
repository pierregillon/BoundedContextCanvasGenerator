using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using Microsoft.Extensions.Logging;

namespace BoundedContextCanvasGenerator.Infrastructure.Monitoring;

public class MonitorCanvasSettingsRepository : ICanvasSettingsRepository
{
    private readonly ICanvasSettingsRepository _decorated;
    private readonly ILogger<MonitorCanvasSettingsRepository> _logger;

    public MonitorCanvasSettingsRepository(ICanvasSettingsRepository decorated, ILogger<MonitorCanvasSettingsRepository> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public Task<ICanvasSettings> Get(CanvasSettingsPath canvasSettingsPath)
    {
        _logger.LogInformation($"Loading {nameof(canvasSettingsPath).ToReadableSentence()} : {canvasSettingsPath.Value}");
        return _decorated.Get(canvasSettingsPath);
    }
}