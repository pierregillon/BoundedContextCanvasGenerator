using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.Extensions.Logging;

namespace BoundedContextCanvasGenerator.Infrastructure.Monitoring
{
    public class MonitorBoundedContextCanvasAnalyser : IBoundedContextCanvasAnalyser
    {
        private readonly IBoundedContextCanvasAnalyser _decorated;
        private readonly ILogger<MonitorBoundedContextCanvasAnalyser> _logger;

        public MonitorBoundedContextCanvasAnalyser(IBoundedContextCanvasAnalyser decorated, ILogger<MonitorBoundedContextCanvasAnalyser> logger)
        {
            _decorated = decorated;
            _logger = logger;
        }
        public async Task<BoundedContextCanvas> Analyse(SolutionPath solutionPath, ICanvasSettings settings)
        {
            _logger.LogInformation("Starting type definition extraction");
            var extraction = await this._decorated.Analyse(solutionPath, settings);
            _logger.LogInformation($"Commands: {extraction.Commands.Count}, Domain events: {extraction.DomainEvents.Count}");
            return extraction;
        }
    }
}
