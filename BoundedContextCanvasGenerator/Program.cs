using System.Runtime.CompilerServices;
using BoundedContextCanvasGenerator;
using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using CommandLine;

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
    var solutionName = new SolutionName(options.SolutionFilePath!);

    var configuration = await new ConfigurationFactory(new YamlDotNetFileReader()).Build(options.ConfigurationFilePath);

    var generator = new ReadmeGenerator(new SourceCodeAnalyserTypeDefinitionRepository(), configuration);

    var readmeContent = await generator.Generate(solutionName);

    await File.WriteAllTextAsync(options.OutputFilePath!, readmeContent);
}