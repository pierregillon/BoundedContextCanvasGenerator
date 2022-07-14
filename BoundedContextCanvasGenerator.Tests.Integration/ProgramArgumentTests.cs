using System;
using BoundedContextCanvasGenerator.Tests.Integration.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Integration;

public class ProgramArgumentTests
{
    private readonly string[] NO_ARGS = Array.Empty<string>();
    private readonly ProgramRunner<Program> _program;

    public ProgramArgumentTests() => _program = new ProgramRunner<Program>();

    [Fact]
    public void Start_generator_without_arguments()
    {
        (string Output, string Error) consoleResult = _program.Run(NO_ARGS);

        consoleResult
            .Output
            .Should()
            .Contain("No solution file provided");
    }
}