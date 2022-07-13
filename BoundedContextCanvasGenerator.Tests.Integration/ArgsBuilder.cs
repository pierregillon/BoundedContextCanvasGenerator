using System;
using System.IO;

namespace BoundedContextCanvasGenerator.Tests.Integration;

public record ArgsBuilder
{
    private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    private string? _solutionAbsolutePath;
    private string _outputAbsolutePath = Path.Combine(BaseDirectory, $"{Guid.NewGuid()}.md");

    public string OutputAbsolutePath => _outputAbsolutePath;
    public bool OutputFileHasBeenCustomized { private set; get; }


    public ArgsBuilder WithSolution(string relativePath)
    {
        this._solutionAbsolutePath = GetAbsoluteSolutionPath(relativePath);
        return this;
    }

    public ArgsBuilder WithOutputFile(string path)
    {
        this._outputAbsolutePath = Path.Combine(BaseDirectory, path);
        this.OutputFileHasBeenCustomized = true;
        return this;
    }

    public string[] Build()
    {
        return new[] {
            "--solution",
            _solutionAbsolutePath ?? throw new InvalidOperationException("Solution path is mandatory"),
            "--output",
            _outputAbsolutePath
        };
    }

    private static string GetAbsoluteSolutionPath(string relativeSolutionPath)
        => Path.Combine(BaseDirectory, "..", "..", "..", "..", "SolutionExample", relativeSolutionPath);
}