using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types;
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

    public IAsyncEnumerable<TypeDefinition> GetAll(SolutionPath path)
    {
        _logger.LogInformation($"Start scanning {nameof(SolutionPath).ToReadableSentence()} : {path.Value}");
        return _decorated.GetAll(path);
    }
}