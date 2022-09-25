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

        string expected = $"## {sectionName}";

        plainText
            .Should()
            .NotContain(expected);
    }

    [Fact]
    public async Task Generating_canvas_with_context_definition()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"name: Catalog
definition:
    description: Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated new items.
    strategic_classification:
        domain: core
        business_model: revenue_generator
        evolution: commodity
    domain_role:
        name: gateway context
        description: Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.
    ")
            .Execute();

        const string expected =
@"# Catalog

## Definition

### Description

> Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated new items.

### Strategic classification [(?)](https://github.com/ddd-crew/bounded-context-canvas#strategic-classification)

| Domain                                         | Business Model                                         | Evolution                                             |
| ---------------------------------------------- | ------------------------------------------------------ | ----------------------------------------------------- |
| *Core domain*<br/>(a key strategic initiative) | *Revenue generator*<br/>(people pay directly for this) | *Commodity*<br/>(highly\-standardised versions exist) |

### Domain role [(?)](https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md): *gateway context*

Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.";
        plainText
            .Should()
            .Contain(expected);
    }

    [Fact]
    public async Task Generating_BCC_with_ubiquitous_language_configuration_lists_context_specific_terminology()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"ubiquitous_language:
    selector: concrete class implementing '.*IAggregateRoot<.*>'
")
            .Execute();

        const string expected =
@"## Ubiquitous language (Context\-specific domain terminology)

| Catalog                                                                                             | Catalog item                                                                               |
| --------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| An enumeration of items to purchase. It is systematically described and target a specific audience. | An item of a catalog. It is the minimum unit to purchase. The price includes the currency. |";
        plainText
            .Should()
            .Contain(expected);
    }

    [Fact]
    public async Task Generating_BCC_with_inbound_communication_configuration_lists_commands_collaborators_and_policies()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"inbound_communication:
    commands: 
        selector: class implementing '.*ICommand$'
    collaborators:
        - name: WebApp
          type: front
          selector: class named '.*Controller$'
    policies:
        - method_attribute_pattern: 'Fact'
")
            .Execute();

        const string expected =
@"## Inbound communication

### Catalog

---

```mermaid
flowchart LR
    classDef commands fill:#352ef722, stroke:#352ef7;
    classDef frontCollaborators fill:#f72ef022, stroke:#f72ef0;
    classDef policies fill:#E9E70522, stroke:#E9E705, font-style:italic;
    CatalogApplicationCatalogDeleteCatalogCommand[""Delete catalog""]
    class CatalogApplicationCatalogDeleteCatalogCommand commands;
    CatalogApplicationCatalogDeleteCatalogCommandWebAppCollaborator>""Web app""]
    class CatalogApplicationCatalogDeleteCatalogCommandWebAppCollaborator frontCollaborators;
    CatalogApplicationCatalogRegisterNewCatalogCommand[""Register new catalog""]
    class CatalogApplicationCatalogRegisterNewCatalogCommand commands;
    CatalogApplicationCatalogRegisterNewCatalogCommandWebAppCollaborator>""Web app""]
    class CatalogApplicationCatalogRegisterNewCatalogCommandWebAppCollaborator frontCollaborators;
    CatalogApplicationCatalogRegisterNewCatalogCommandPolicies[/""A catalog name is unique<br/>Registering a catalog raises registered event""/]
    class CatalogApplicationCatalogRegisterNewCatalogCommandPolicies policies;
    CatalogApplicationCatalogDeleteCatalogCommandWebAppCollaborator --> CatalogApplicationCatalogDeleteCatalogCommand
    CatalogApplicationCatalogRegisterNewCatalogCommandWebAppCollaborator --> CatalogApplicationCatalogRegisterNewCatalogCommand
    CatalogApplicationCatalogRegisterNewCatalogCommand --- CatalogApplicationCatalogRegisterNewCatalogCommandPolicies
```

### Items

---

```mermaid
flowchart LR
    classDef commands fill:#352ef722, stroke:#352ef7;
    classDef frontCollaborators fill:#f72ef022, stroke:#f72ef0;
    CatalogApplicationItemsAddItemToCatalogCommand[""Add item to catalog""]
    class CatalogApplicationItemsAddItemToCatalogCommand commands;
    CatalogApplicationItemsAddItemToCatalogCommandWebAppCollaborator>""Web app""]
    class CatalogApplicationItemsAddItemToCatalogCommandWebAppCollaborator frontCollaborators;
    CatalogApplicationItemsAdjustItemPriceCommand[""Adjust item price""]
    class CatalogApplicationItemsAdjustItemPriceCommand commands;
    CatalogApplicationItemsAdjustItemPriceCommandWebAppCollaborator>""Web app""]
    class CatalogApplicationItemsAdjustItemPriceCommandWebAppCollaborator frontCollaborators;
    CatalogApplicationItemsEntitleItemCommand[""Entitle item""]
    class CatalogApplicationItemsEntitleItemCommand commands;
    CatalogApplicationItemsEntitleItemCommandWebAppCollaborator>""Web app""]
    class CatalogApplicationItemsEntitleItemCommandWebAppCollaborator frontCollaborators;
    CatalogApplicationItemsReduceItemQuantityCommand[""Reduce item quantity""]
    class CatalogApplicationItemsReduceItemQuantityCommand commands;
    CatalogApplicationItemsRemoveFromCatalogCommand[""Remove from catalog""]
    class CatalogApplicationItemsRemoveFromCatalogCommand commands;
    CatalogApplicationItemsRemoveFromCatalogCommandWebAppCollaborator>""Web app""]
    class CatalogApplicationItemsRemoveFromCatalogCommandWebAppCollaborator frontCollaborators;
    CatalogApplicationItemsAddItemToCatalogCommandWebAppCollaborator --> CatalogApplicationItemsAddItemToCatalogCommand
    CatalogApplicationItemsAdjustItemPriceCommandWebAppCollaborator --> CatalogApplicationItemsAdjustItemPriceCommand
    CatalogApplicationItemsEntitleItemCommandWebAppCollaborator --> CatalogApplicationItemsEntitleItemCommand
    CatalogApplicationItemsRemoveFromCatalogCommandWebAppCollaborator --> CatalogApplicationItemsRemoveFromCatalogCommand
```";
        plainText
            .Should()
            .Contain(expected);
    }

    [Fact]
    public async Task Generating_BCC_with_inbound_communication_configuration_lists_commands_and_events_through_handler()
    {
        var plainText = await A
            .Generator()
            .TargetingSolution(ExampleSolution)
            .WithConfiguration(
@"inbound_communication:
    commands: 
        selector: class implementing '.*ICommand$'
        handler: 
            selector: class implementing '.*ICommandHandler<.*>$'
            link: T -> .*ICommandHandler<T>$
    domain_events:
        selector: class implementing 'IDomainEvent'
        handler: 
            selector: class implementing '.*IDomainEventListener<.*>$'
            link: T -> .*IDomainEventListener<T>$
    integration_events:
        selector: class implementing 'IIntegrationEvent'
")
            .Execute();

        const string expected =
@"## Inbound communication

### Catalog

---

```mermaid
flowchart LR
    classDef commands fill:#352ef722, stroke:#352ef7;
    classDef domainEvents fill:#f7962e22, stroke:#f7962e;
    classDef integrationEvents fill:#f7962e22, stroke:#f7962e;
    CatalogApplicationCatalogDeleteCatalogCommand[""Delete catalog""]
    class CatalogApplicationCatalogDeleteCatalogCommand commands;
    CatalogDomainCatalogEventsCatalogDeleted[""Catalog deleted""]
    class CatalogDomainCatalogEventsCatalogDeleted domainEvents;
    CatalogApplicationCatalogRegisterNewCatalogCommand[""Register new catalog""]
    class CatalogApplicationCatalogRegisterNewCatalogCommand commands;
    CatalogDomainCatalogEventsCatalogRegistered[""Catalog registered""]
    class CatalogDomainCatalogEventsCatalogRegistered domainEvents;
    CatalogInfrastructureCatalogCatalogCreatedIntegrationEvent[""Catalog created""]
    class CatalogInfrastructureCatalogCatalogCreatedIntegrationEvent integrationEvents;
    CatalogApplicationCatalogDeleteCatalogCommand -.-> CatalogDomainCatalogEventsCatalogDeleted
    CatalogApplicationCatalogRegisterNewCatalogCommand -.-> CatalogDomainCatalogEventsCatalogRegistered
    CatalogDomainCatalogEventsCatalogRegistered -.-> CatalogInfrastructureCatalogCatalogCreatedIntegrationEvent
```

### Items

---

```mermaid
flowchart LR
    classDef commands fill:#352ef722, stroke:#352ef7;
    classDef domainEvents fill:#f7962e22, stroke:#f7962e;
    CatalogApplicationItemsAddItemToCatalogCommand[""Add item to catalog""]
    class CatalogApplicationItemsAddItemToCatalogCommand commands;
    CatalogDomainItemsEventsCatalogItemAdded[""Catalog item added""]
    class CatalogDomainItemsEventsCatalogItemAdded domainEvents;
    CatalogApplicationItemsAdjustItemPriceCommand[""Adjust item price""]
    class CatalogApplicationItemsAdjustItemPriceCommand commands;
    CatalogDomainItemsEventsCatalogItemPriceAdjusted[""Catalog item price adjusted""]
    class CatalogDomainItemsEventsCatalogItemPriceAdjusted domainEvents;
    CatalogApplicationItemsEntitleItemCommand[""Entitle item""]
    class CatalogApplicationItemsEntitleItemCommand commands;
    CatalogDomainItemsEventsCatalogItemEntitled[""Catalog item entitled""]
    class CatalogDomainItemsEventsCatalogItemEntitled domainEvents;
    CatalogApplicationItemsReduceItemQuantityCommand[""Reduce item quantity""]
    class CatalogApplicationItemsReduceItemQuantityCommand commands;
    CatalogDomainItemsEventsCatalogItemQuantityAdjusted[""Catalog item quantity adjusted""]
    class CatalogDomainItemsEventsCatalogItemQuantityAdjusted domainEvents;
    CatalogApplicationItemsRemoveFromCatalogCommand[""Remove from catalog""]
    class CatalogApplicationItemsRemoveFromCatalogCommand commands;
    CatalogDomainItemsEventsCatalogItemRemoved[""Catalog item removed""]
    class CatalogDomainItemsEventsCatalogItemRemoved domainEvents;
    CatalogApplicationItemsAddItemToCatalogCommand -.-> CatalogDomainItemsEventsCatalogItemAdded
    CatalogApplicationItemsAdjustItemPriceCommand -.-> CatalogDomainItemsEventsCatalogItemPriceAdjusted
    CatalogApplicationItemsEntitleItemCommand -.-> CatalogDomainItemsEventsCatalogItemEntitled
    CatalogApplicationItemsReduceItemQuantityCommand -.-> CatalogDomainItemsEventsCatalogItemQuantityAdjusted
    CatalogApplicationItemsRemoveFromCatalogCommand -.-> CatalogDomainItemsEventsCatalogItemRemoved
```";
        plainText
            .Should()
            .Contain(expected);
    }
}