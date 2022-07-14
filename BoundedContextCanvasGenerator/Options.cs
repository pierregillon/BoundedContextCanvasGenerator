using CommandLine;

namespace BoundedContextCanvasGenerator;

public class Options
{
    [Option("solution", Required = true, HelpText = "The solution to analyze.")]
    public string? SolutionFilePath { get; set; }

    [Option("output", Required = true, HelpText = "The output readme file.")]
    public string? OutputFilePath { get; set; }
    
    [Option("configuration", Required = false, HelpText = "The yaml configuration file.")]
    public string? ConfigurationFilePath { get; set; }
}