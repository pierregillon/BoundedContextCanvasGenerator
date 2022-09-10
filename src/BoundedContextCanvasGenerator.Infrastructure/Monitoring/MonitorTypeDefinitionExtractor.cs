using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.Extensions.Logging;

namespace BoundedContextCanvasGenerator.Infrastructure.Monitoring;

public class MonitorTypeDefinitionExtractor : ITypeDefinitionExtractor
{
    private readonly ITypeDefinitionExtractor _decorated;
    private readonly ILogger<MonitorTypeDefinitionExtractor> _logger;

    public MonitorTypeDefinitionExtractor(ITypeDefinitionExtractor decorated, ILogger<MonitorTypeDefinitionExtractor> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task<TypeDefinitionExtract> Extract(SolutionPath solutionPath, ICanvasSettings settings)
    {
        _logger.LogInformation("Starting type definition extraction");
        var extraction = await _decorated.Extract(solutionPath, settings);
        _logger.LogInformation($"Commands: {extraction.Commands.Count}, Domain events: {extraction.DomainEvents.Count}");
        return extraction;
    }
}