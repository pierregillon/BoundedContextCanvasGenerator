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

    public async Task<string> Generate(TypeDefinitionExtraction extraction, ICanvasSettings canvasSettings)
    {
        _logger.LogInformation("Generating markdown from extraction");
        var markdown = await _decorated.Generate(extraction, canvasSettings);
        _logger.LogInformation($"Markdown generated: {markdown.Length} char");
        return markdown;
    }
}