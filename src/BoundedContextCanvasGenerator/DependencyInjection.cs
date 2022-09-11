using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;
using BoundedContextCanvasGenerator.Infrastructure.Monitoring;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BoundedContextCanvasGenerator;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        services
            .AddScoped<RenderBoundedContextCanvas>()
            .AddScoped<GenerateBoundedContextCanvasFromSolutionPath>()
            .AddScoped<BoundedContextCanvasAnalyser>()
            .AddScoped<TypeDefinitionFactory>()
            .AddScoped<ITypeDefinitionRepository, SourceCodeAnalyserTypeDefinitionRepository>()
            .AddScoped<TypeDefinitionFilter>()
            .AddScoped<ICanvasSettingsRepository, YamlFileCanvasSettingsRepository>()
            .AddScoped<IBoundedContextCanvasRenderer, GrynwaldMarkdownBoundedContextCanvasRenderer>()
            ;
        
        return services;
    }

    public static IServiceCollection RegisterMonitoring(this IServiceCollection services)
    {
        services
            .Decorate<ITypeDefinitionRepository, MonitorTypeDefinitionRepository>()
            .Decorate<ICanvasSettingsRepository, MonitorCanvasSettingsRepository>()
            .Decorate<IBoundedContextCanvasRenderer, MonitorBoundedContextCanvasRenderer>()
            .AddLogging(x => x.AddConsole())
            ;

        return services;
    }
}