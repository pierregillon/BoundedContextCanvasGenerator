using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using A = BoundedContextCanvasGenerator.Tests.Integration.Utils.BoundedContextCanvasGeneratorProgram;

namespace BoundedContextCanvasGenerator.Tests.Integration;

public class BoundedContextGeneratorTests
{
    private const string ExampleSolution = "Example\\Example.sln";

    [Fact]
    public async Task Generating_BCC_twice_returns_the_same_result()
    {
        var builder = A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .OutputtingFile("results.md");

        var firstGeneration = await builder.Execute();
        var secondGeneration = await builder.Execute();

        firstGeneration.Should().Be(secondGeneration);
    }

    [Theory]
    [InlineData("Commands")]
    [InlineData("Events")]
    public async Task Generating_BCC_without_configuration_does_not_generate_sections(string sectionName)
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithEmptyConfiguration()
            .Execute();

        const string commandsSection = "## {sectionName}";

        plainText
            .Should()
            .NotContain(commandsSection);
    }

    [Fact]
    public async Task Generating_BCC_with_commands_configuration_lists_commands_matching_predicates()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"commands:
    type: 'class'
    implementing:
        pattern: '.*ICommand$'")
            .Execute();

        const string commandsSection = 
@"## Commands
- Catalog.Application.Items.AddItemToCatalogCommand
- Catalog.Application.Items.AdjustItemPriceCommand
- Catalog.Application.Items.EntitleItemCommand
- Catalog.Application.Items.RemoveFromCatalogCommand
";
        plainText
            .Should()
            .Contain(commandsSection);
    }

    [Fact]
    public async Task Generating_BCC_with_domain_events_configuration_lists_them_that_matches_predicates()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"domainEvents:
    type: 'class'
    implementing:
        pattern: '.*IDomainEvent$'")
            .Execute();

        const string commandsSection =
@"## Domain events
- Catalog.Domain.Items.Events.CatalogItemAdded
- Catalog.Domain.Items.Events.CatalogItemEntitled
- Catalog.Domain.Items.Events.CatalogItemPriceAdjusted
- Catalog.Domain.Items.Events.CatalogItemRemoved
";
        plainText
            .Should()
            .Contain(commandsSection);
    }
}