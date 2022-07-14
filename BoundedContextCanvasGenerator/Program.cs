using System.Runtime.CompilerServices;
using BoundedContextCanvasGenerator;
using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("BoundedContextCanvasGenerator.Tests.Integration")]

if (!args.Any()) {
    Console.WriteLine("No solution file provided");
    return;
}

var parsedResults = Parser.Default.ParseArguments<Options>(args);

await parsedResults.MapResult(
    RunApplicationAsync,
    _ => Task.FromResult(1)
);


static async Task RunApplicationAsync(Options options)
{
    var serviceProvider = new ServiceCollection()
        .AddScoped<MarkdownBoundedContextCanvasGenerator>()
        .AddScoped<ITypeDefinitionRepository, SourceCodeAnalyserTypeDefinitionRepository>()
        .AddScoped<IConfigurationRepository, YamlFileConfigurationRepository>()
        .BuildServiceProvider();

    var generator = serviceProvider.GetRequiredService<MarkdownBoundedContextCanvasGenerator>();

    var markdown = await generator.Generate(
        new SolutionPath(options.SolutionFilePath), 
        new ConfigurationPath(options.ConfigurationFilePath)
    );

    await File.WriteAllTextAsync(options.OutputFilePath!, markdown);
}