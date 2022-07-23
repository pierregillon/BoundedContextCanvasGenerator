using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Application.Markdown;
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
            .AddScoped<MarkdownBoundedContextCanvasGenerator>()
            .AddScoped<ITypeDefinitionRepository, SourceCodeAnalyserTypeDefinitionRepository>()
            .AddScoped<ICanvasSettingsRepository, YamlFileCanvasSettingsRepository>()
            .AddScoped<ITypeDefinitionExtractor, TypeDefinitionExtractor>()
            .AddScoped<IMarkdownGenerator, GrynwaldMarkdownGenerator>()
            .AddScoped<TypeDefinitionFactory>()
            ;
        
        return services;
    }

    public static IServiceCollection RegisterMonitoring(this IServiceCollection services)
    {
        services
            .Decorate<ITypeDefinitionRepository, MonitorTypeDefinitionRepository>()
            .Decorate<ICanvasSettingsRepository, MonitorCanvasSettingsRepository>()
            .Decorate<ITypeDefinitionExtractor, MonitorTypeDefinitionExtractor>()
            .Decorate<IMarkdownGenerator, MonitorMarkdownGenerator>()
            .AddLogging(x => x.AddConsole())
            ;

        return services;
    }
}