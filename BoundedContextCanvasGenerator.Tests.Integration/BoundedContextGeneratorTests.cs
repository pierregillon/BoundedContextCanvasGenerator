using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
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

    [Fact]
    public async Task Generating_BCC_with_solution_path_only_output_result_in_default_file()
    {
        var rootDirectory = Path.GetDirectoryName(A.GetAbsoluteSolutionPath(ExampleSolution))!;
        var settingsPath = Path.Combine(rootDirectory, "bounded_context_canvas_settings.yaml");
        var resultPath = Path.Combine(rootDirectory, "bounded_context_canvas.md");

        if (File.Exists(resultPath)) {
            File.Delete(resultPath);
        }

        _ = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .NoOutputFileDefined()
            .Execute();

        File.Exists(resultPath).Should().BeTrue();
    }

    [Fact]
    public async Task Generating_BCC_with_solution_path_only_use_settings_of_default_file()
    {
        using var scope = new AssertionScope();

        var rootDirectory = Path.GetDirectoryName(A.GetAbsoluteSolutionPath(ExampleSolution))!;
        var settingsPath = Path.Combine(rootDirectory, "bounded_context_canvas_settings.yaml");
        var tempSettingsPath = settingsPath + "_backup";

        if (File.Exists(settingsPath)) {
            File.Move(settingsPath, tempSettingsPath);
        }

        Func<Task> tryGenerating = () => A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .NoOutputFileDefined()
            .Execute();

        await tryGenerating
            .Should()
            .ThrowAsync<TargetInvocationException>();

        if (File.Exists(tempSettingsPath)) {
            File.Move(tempSettingsPath, settingsPath);
        }
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
    type: class
    implementing:
        pattern: .*IDomainEvent$")
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

    [Fact]
    public async Task Generating_BCC_with_ubquitous_language_configuration_lists_context_specific_terminology()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"ubiquitous_language:
    type: class
    modifiers:
        - concrete
    implementing:
        pattern: .*IAggregateRoot<.*>")
            .Execute();

        const string commandsSection =
@"## Ubiquitous language (Context-specific domain terminology)
| Catalog item |
| ----- |
| An item of a catalog. It is the minimum unit to purchase. The price includes the currency. |";
        plainText
            .Should()
            .Contain(commandsSection);
    }
}