using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Integration;

public class BoundedContextGeneratorTests
{
    private readonly string[] NO_ARGS = Array.Empty<string>();
    private const string ExampleSolution = "Example\\Example.sln";

    [Fact]
    public void Start_generator_without_arguments()
    {
        var consoleResult = RunWithArgs(NO_ARGS);

        consoleResult
            .Should()
            .Contain("No solution file provided");
    }

    [Fact]
    public async Task Generating_BCC_twice_returns_the_same_result()
    {
        var firstGeneration = await GenerateBoundedContextCanvas(args => args.WithSolution(ExampleSolution).WithOutputFile("results.md"));
        var secondGeneration = await GenerateBoundedContextCanvas(args => args.WithSolution(ExampleSolution).WithOutputFile("results.md"));

        firstGeneration.Should().Be(secondGeneration);
    }

    [Fact]
    public async Task Generating_correct_commands()
    {
        var plainText = await GenerateBoundedContextCanvas(args => args.WithSolution(ExampleSolution));

        const string expectedCommandSection = 
@"## Commands
- Catalog.Application.Items.AddItemToCatalogCommand
- Catalog.Application.Items.AdjustItemPriceCommand
- Catalog.Application.Items.EntitleItemCommand
- Catalog.Application.Items.RemoveFromCatalogCommand
";
        plainText
            .Should()
            .Contain(expectedCommandSection);
    }

    private static async Task<string> GenerateBoundedContextCanvas(Func<ArgsBuilder, ArgsBuilder> configure)
    {
        var argsBuilder = new ArgsBuilder();

        argsBuilder = configure(argsBuilder);

        RunWithArgs(argsBuilder.Build());

        var results = await File.ReadAllTextAsync(argsBuilder.OutputAbsolutePath);

        if (!argsBuilder.OutputFileHasBeenCustomized)
        {
            File.Delete(argsBuilder.OutputAbsolutePath);
        }

        return results;
    }

    private static string RunWithArgs(params string[] args)
    {
        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);
        Console.SetOut(writer);
        Execute(args);
        return stringBuilder.ToString();
    }

    private static void Execute(string[] args)
    {
        var programAssembly = Assembly.GetAssembly(typeof(Program));
        if (programAssembly is null)
        {
            throw new InvalidOperationException("Unable to find the bounded context generator program");
        }

        programAssembly.EntryPoint?.Invoke(null, new object?[] {args});
    }
}