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

    IGeneratorConfiguration configuration;
    if (options.ConfigurationFilePath is null)
    {
        configuration = new DefaultGeneratorConfiguration();
    }
    else {
        configuration = await new YamlFileConfigurationRepository(options.ConfigurationFilePath).Get();
    }

    var generator = new ReadmeGenerator(new SourceCodeAnalyserTypeDefinitionRepository(), configuration);

    var readmeContent = await generator.Generate(solutionName);

    await File.WriteAllTextAsync(options.OutputFilePath!, readmeContent);
}