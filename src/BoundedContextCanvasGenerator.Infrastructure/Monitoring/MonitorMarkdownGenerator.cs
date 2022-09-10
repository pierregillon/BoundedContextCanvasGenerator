using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Application.Markdown;
using BoundedContextCanvasGenerator.Domain.Configuration;
using Microsoft.Extensions.Logging;

namespace BoundedContextCanvasGenerator.Infrastructure.Monitoring;

public class MonitorMarkdownGenerator : IMarkdownGenerator
{
    private readonly IMarkdownGenerator _decorated;
    private readonly ILogger<MonitorMarkdownGenerator> _logger;

    public MonitorMarkdownGenerator(IMarkdownGenerator decorated, ILogger<MonitorMarkdownGenerator> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task<string> Render(BoundedContextCanvas boundedContextCanvas)
    {
        _logger.LogInformation("Generating markdown from extraction");
        var markdown = await _decorated.Render(boundedContextCanvas);
        _logger.LogInformation($"Markdown generated: {markdown.Length} char");
        return markdown;
    }
}