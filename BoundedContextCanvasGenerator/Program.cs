using System.Runtime.CompilerServices;
using BoundedContextCanvasGenerator;
using CommandLine;
using LivingDocumentation.Domain;
using LivingDocumentation.Infrastructure;

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

    var configuration = await new ConfigurationFactory().Build(options.ConfigurationFilePath);

    var generator = new ReadmeGenerator(new SourceCodeAnalyserTypeDefinitionRepository(), configuration);

    var readmeContent = await generator.Generate(solutionName);

    await File.WriteAllTextAsync(options.OutputFilePath!, readmeContent);
}

public class Options
{
    [Option("solution", Required = true, HelpText = "The solution to analyze.")]
    public string? SolutionFilePath { get; set; }

    [Option("output", Required = true, HelpText = "The output readme file.")]
    public string? OutputFilePath { get; set; }
    
    [Option("configuration", Required = false, HelpText = "The yaml configuration file.")]
    public string? ConfigurationFilePath { get; set; }
}