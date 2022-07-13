using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Integration
{
    public class BoundedContextGeneratorTests
    {
        private readonly string[] NO_ARGS = Array.Empty<string>();
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string ExampleSolution = "Example\\Example.sln";

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
            var firstGeneration = await GenerateSolution(ExampleSolution, "results.md");
            var secondGeneration = await GenerateSolution(ExampleSolution, "results.md");

            firstGeneration.Should().Be(secondGeneration);
        }

        [Fact]
        public async Task Generating_correct_commands()
        {
            var textResult = await GenerateSolution(ExampleSolution);

            var expectedCommandSection = 
@"## Commands
- Catalog.Application.Items.AddItemToCatalogCommand
- Catalog.Application.Items.AdjustItemPriceCommand
- Catalog.Application.Items.EntitleItemCommand
- Catalog.Application.Items.RemoveFromCatalogCommand
";
            textResult
                .Should()
                .Contain(expectedCommandSection);
        }

        private static async Task<string> GenerateSolution(string relativeSolutionPath, string? outputFileName = default)
        {
            var outputFile = GetAbsoluteOutputFilePath(outputFileName);
            var solutionPath = GetAbsoluteSolutionPath(relativeSolutionPath);

            RunWithArgs("--solution", solutionPath, "--output", outputFile);

            var results = await File.ReadAllTextAsync(outputFile);

            if (outputFileName is null)
            {
                File.Delete(outputFile);
            }

            return results;
        }

        private static string GetAbsoluteOutputFilePath(string? outputFileName) 
            => Path.Combine(BaseDirectory, outputFileName ?? $"{Guid.NewGuid()}.md");

        private static string GetAbsoluteSolutionPath(string relativeSolutionPath) 
            => Path.Combine(BaseDirectory, "..", "..", "..", "..", "SolutionExample", relativeSolutionPath);

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
    
}