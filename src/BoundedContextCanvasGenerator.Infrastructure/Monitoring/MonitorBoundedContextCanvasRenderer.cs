using BoundedContextCanvasGenerator.Domain.BC;
using Microsoft.Extensions.Logging;

namespace BoundedContextCanvasGenerator.Infrastructure.Monitoring;

public class MonitorBoundedContextCanvasRenderer : IBoundedContextCanvasRenderer
{
    private readonly IBoundedContextCanvasRenderer _decorated;
    private readonly ILogger<MonitorBoundedContextCanvasRenderer> _logger;

    public MonitorBoundedContextCanvasRenderer(IBoundedContextCanvasRenderer decorated, ILogger<MonitorBoundedContextCanvasRenderer> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task<Bytes> Render(BoundedContextCanvas boundedContextCanvas)
    {
        _logger.LogInformation("Generating markdown from extraction");
        var markdown = await _decorated.Render(boundedContextCanvas);
        _logger.LogInformation($"Markdown generated: {markdown.Length} char");
        return markdown;
    }
}