using System.Runtime.CompilerServices;
using BoundedContextCanvasGenerator;
using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.Types;
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
        .RegisterApplication()
        .RegisterMonitoring()
        .BuildServiceProvider();

    var generator = serviceProvider.GetRequiredService<MarkdownBoundedContextCanvasGenerator>();

    var solutionPath = new SolutionPath(options.SolutionPath);
    var canvasSettingsPath = options.GetCanvasSettingsPathOrDefault();
    var outputPath = options.GetOutputPathOrDefault();

    var markdown = await generator.Generate(
        solutionPath,
        canvasSettingsPath
    );

    await WriteAllText(outputPath, markdown);
}

static Task WriteAllText(OutputPath outputFilePath, string content) 
    => File.WriteAllTextAsync(outputFilePath.Value, content);