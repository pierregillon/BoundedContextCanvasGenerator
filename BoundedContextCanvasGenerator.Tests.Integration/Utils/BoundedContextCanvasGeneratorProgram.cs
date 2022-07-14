using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BoundedContextCanvasGenerator.Tests.Integration.Utils;

public record BoundedContextCanvasGeneratorProgram
{
    private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private readonly ProgramRunner<Program> _runner = new();

    private string? _solutionAbsolutePath;
    private string? _outputAbsolutePath;
    private string? _configurationAbsolutePath;
    private bool _outputFileHasBeenCustomized;

    public static BoundedContextCanvasGeneratorProgram Generator() 
        => new BoundedContextCanvasGeneratorProgram().OutputtingFile($"{Guid.NewGuid()}.md");

    public BoundedContextCanvasGeneratorProgram TargetingSolution(string relativePath)
    {
        this._solutionAbsolutePath = GetAbsoluteSolutionPath(relativePath);
        return this;
    }

    public BoundedContextCanvasGeneratorProgram OutputtingFile(string path)
    {
        this._outputAbsolutePath = Path.Combine(BaseDirectory, path);
        this._outputFileHasBeenCustomized = true;
        return this;
    }

    public BoundedContextCanvasGeneratorProgram WithConfiguration(string configurationYaml)
    {
        var filePath = Path.Combine(BaseDirectory, $"{Guid.NewGuid()}.yaml");
        File.WriteAllTextAsync(filePath, configurationYaml);
        _configurationAbsolutePath = filePath;
        return this;
    }

    public BoundedContextCanvasGeneratorProgram WithEmptyConfiguration() => WithConfiguration(string.Empty);

    public async Task<string> Execute()
    {
        try
        {
            (string Output, string Error) output = this._runner.Run(BuildArgs());

            if (!File.Exists(this._outputAbsolutePath))
            {
                throw new InvalidOperationException($"Nothing has been generated.\r\n{output.Output}");
            }

            var results = await File.ReadAllTextAsync(this._outputAbsolutePath);

            return results;
        }
        finally
        {
            this.ClearFiles();
        }
    }

    private IEnumerable<string> BuildArgs()
    {
        yield return "--solution";
        yield return _solutionAbsolutePath ?? throw new InvalidOperationException("Solution path is mandatory");
        yield return "--output";
        yield return _outputAbsolutePath;

        if (!string.IsNullOrWhiteSpace(this._configurationAbsolutePath))
        {
            yield return "--configuration";
            yield return _configurationAbsolutePath;
        }

    }

    private static string GetAbsoluteSolutionPath(string relativeSolutionPath)
        => Path.Combine(BaseDirectory, "..", "..", "..", "..", "SolutionExample", relativeSolutionPath);

    public void ClearFiles()
    {
        if (!_outputFileHasBeenCustomized && File.Exists(_outputAbsolutePath)) {
            File.Delete(_outputAbsolutePath);
        }
        if (!string.IsNullOrWhiteSpace(this._configurationAbsolutePath) && File.Exists(this._configurationAbsolutePath)) {
            File.Delete(this._configurationAbsolutePath);
        }
    }
}