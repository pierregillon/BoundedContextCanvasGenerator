using BoundedContextCanvasGenerator.Domain.Configuration;
using CommandLine;

namespace BoundedContextCanvasGenerator;

public class Options
{
    [Option("solution", Required = true, HelpText = "The solution to analyze.")]
    public string? SolutionPath { get; set; }

    [Option("output", Required = false, HelpText = "The output readme file.")]
    public string? OutputPath { get; set; }
    
    [Option("configuration", Required = false, HelpText = "The yaml configuration file.")]
    public string? CanvasSettingsPath { get; set; }

    public CanvasSettingsPath GetCanvasSettingsPathOrDefault() =>
        this.CanvasSettingsPath is not null
            ? Domain.Configuration.CanvasSettingsPath.FromPath(this.CanvasSettingsPath)
            : Domain.Configuration.CanvasSettingsPath.FromSolutionPath(this.SolutionPath!);

    public OutputPath GetOutputPathOrDefault() => 
        OutputPath is not null 
            ? BoundedContextCanvasGenerator.OutputPath.FromPath(OutputPath) 
            : BoundedContextCanvasGenerator.OutputPath.FromSolutionPath(this.SolutionPath!);
}