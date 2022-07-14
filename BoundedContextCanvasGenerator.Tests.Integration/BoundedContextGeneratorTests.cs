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

        string commandsSection = $"## {sectionName}";

        plainText
            .Should()
            .NotContain(commandsSection);
    }

    [Fact]
    public async Task Generating_canvas_with_context_definition()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"name: 'Catalog'
definition:
    description: 'Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated new items.'
    strategic_classification:
        domain: 'core'
        business_model: 'revenue_generator'
        evolution: 'commodity'
    domain_role:
        name: 'gateway context'
        description: 'Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.'
    ")
            .Execute();

        const string definitionSection =
@"# Catalog

## Definition

### Description
> Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated new items.

### Strategic classification [(?)](https://github.com/ddd-crew/bounded-context-canvas#strategic-classification)
| Domain | Business Model | Evolution |
| ------------ | ------------ | ------------ |
| *Core domain*<br/>(a key strategic initiative) | *Revenue generator*<br/>(people pay directly for this) | *Commodity*<br/>(highly-standardised versions exist) |

### Domain role [(?)](https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md): *gateway context*
Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.";
        plainText
            .Should()
            .Contain(definitionSection);
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
@"domain_events:
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