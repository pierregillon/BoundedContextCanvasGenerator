using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using Microsoft.Extensions.Logging;

namespace BoundedContextCanvasGenerator.Infrastructure.Monitoring;

public class MonitorTypeDefinitionRepository : ITypeDefinitionRepository
{
    private readonly ITypeDefinitionRepository _decorated;
    private readonly ILogger<MonitorTypeDefinitionRepository> _logger;

    public MonitorTypeDefinitionRepository(ITypeDefinitionRepository decorated, ILogger<MonitorTypeDefinitionRepository> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<TypeDefinition>> GetAll(SolutionPath path)
    {
        _logger.LogInformation("Loading type definition : START");

        var results = await _decorated.GetAll(path);

        _logger.LogInformation($"Loading type definition : END with {results.Count} elements");

        return results;
    }
}