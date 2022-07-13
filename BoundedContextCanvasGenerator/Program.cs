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
    var solutionName = new SolutionName(options.Solution!);

    var generator = new ReadmeGenerator(new SourceCodeAnalyserTypeDefinitionRepository(), new DefaultGeneratorConfiguration());

    var readmeContent = await generator.Generate(solutionName);

    await File.WriteAllTextAsync(options.Output!, readmeContent);
}

public class Options
{
    [Option("solution", Required = true, HelpText = "The solution to analyze.")]
    public string? Solution { get; set; }

    [Option("output", Required = true, HelpText = "The output readme file.")]
    public string? Output { get; set; }
}

public class DefaultGeneratorConfiguration : IGeneratorConfiguration
{
    public IGeneratorDefinition CommandDefinition => new ImplementsInterfaceMatching(".*ICommand$");
}