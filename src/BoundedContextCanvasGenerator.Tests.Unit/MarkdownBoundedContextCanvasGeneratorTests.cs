using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit
{
    public class MarkdownBoundedContextCanvasGeneratorTests
    {
        private static readonly SolutionPath SomeSolution = new("some");
        private static readonly CanvasSettingsPath SomeCanvasSettingsPath = new("some");

        private readonly MarkdownBoundedContextCanvasGenerator _generator;
        private readonly ITypeDefinitionRepository _typeDefinitionRepository = Substitute.For<ITypeDefinitionRepository>();
        private readonly ICanvasSettingsRepository _canvasSettingsRepository = Substitute.For<ICanvasSettingsRepository>();
        private readonly ICanvasSettings _canvasSettings = Substitute.For<ICanvasSettings>();

        public MarkdownBoundedContextCanvasGeneratorTests()
        {
            var serviceProvider = new ServiceCollection()
                .RegisterApplication()
                .AddScoped(_ => _typeDefinitionRepository)
                .AddScoped(_ => _canvasSettingsRepository)
                .BuildServiceProvider();

            _generator = serviceProvider.GetRequiredService<MarkdownBoundedContextCanvasGenerator>();

            _canvasSettingsRepository
                .Get(Arg.Any<CanvasSettingsPath>())
                .Returns(_canvasSettings);

            _canvasSettings
                .Name
                .Returns(CanvasName.Default);
            
            _canvasSettings
                .Definition
                .Returns(CanvasDefinition.Empty);
            
            _canvasSettings
                .InboundCommunication
                .Returns(InboundCommunication.Empty);

            _canvasSettings
                .DomainEvents
                .Returns(TypeDefinitionPredicates.Empty);

            _canvasSettings
                .UbiquitousLanguage
                .Returns(TypeDefinitionPredicates.Empty);
        }

        [Fact]
        public async Task No_definition_settings_do_not_generate_definition_section()
        {
            var markdown = await GenerateMarkdown();

            markdown.Should().NotContain("## Definition");
        }

        [Fact]
        public async Task Name_settings_generates_section()
        {
            const string name = "Catalog";

            _canvasSettings
                .Name
                .Returns(new CanvasName(name));

            var markdown = await GenerateMarkdown();

            markdown.Should().StartWith(@"# Catalog");
        }

        [Fact]
        public async Task Strategic_classification_generates_section()
        {
            _canvasSettings
                .Definition
                .Returns(new CanvasDefinition(
                    Text.Empty, 
                    new StrategicClassification(DomainType.CoreDomain, BusinessModel.RevenueGenerator, Evolution.Commodity),
                    DomainRole.Empty
                ));

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"### Strategic classification [(?)](https://github.com/ddd-crew/bounded-context-canvas#strategic-classification)

| Domain                                         | Business Model                                         | Evolution                                             |
| ---------------------------------------------- | ------------------------------------------------------ | ----------------------------------------------------- |
| *Core domain*<br/>(a key strategic initiative) | *Revenue generator*<br/>(people pay directly for this) | *Commodity*<br/>(highly\-standardised versions exist) |
");
        }

        [Fact]
        public async Task Domain_role_generates_section()
        {
            _canvasSettings
                .Definition
                .Returns(new CanvasDefinition(
                    Text.Empty,
                    StrategicClassification.Empty,
                    new DomainRole(new Text("gateway context"), new Text("Provide catalog item allowing Basket, Ordering and Payment contexts to properly work."))
                ));

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"### Domain role [(?)](https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md): *gateway context*

Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.
");
        }

        [Fact]
        public async Task Description_settings_generates_section()
        {
            const string description = "Display the product catalog and the items available to purchase.";

            _canvasSettings
                .Definition
                .Returns(new CanvasDefinition(new Text(description), StrategicClassification.Empty, DomainRole.Empty));

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Definition

### Description

> Display the product catalog and the items available to purchase.");
        }

        [Fact]
        public async Task No_domain_events_configuration_do_not_generate_domain_events_section()
        {
            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.MyDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
                A.Class("Some.Namespace.MySecondDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
            });

            var markdown = await GenerateMarkdown();

            markdown.Should().NotContain("## Domain events");
        }

        [Fact]
        public async Task No_domain_events_matching_render_empty_section()
        {
            _canvasSettings
                .DomainEvents
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*IDomainEvent")));

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Domain events

No domain event found
");
        }

        [Fact]
        public async Task Domain_events_matching_pattern_are_listed()
        {
            _canvasSettings
                .DomainEvents
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*IDomainEvent")));

            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.MyDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
                A.Class("Some.Namespace.MySecondDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
            });

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Domain events

- Some.Namespace.MyDomainEvent
- Some.Namespace.MySecondDomainEvent
");
        }

        [Fact]
        public async Task No_ubiquitous_language_render_empty_section()
        {
            _canvasSettings
                .UbiquitousLanguage
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*IAggregateRoot\\<.*\\>")));

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Ubiquitous language (Context\-specific domain terminology)

No ubiquitous language found
");
        }

        [Fact]
        public async Task Ubiquitous_language_renders_class_implementing_interfaces()
        {
            _canvasSettings
                .UbiquitousLanguage
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*IAggregateRoot<.*>")));

            Define(new TypeDefinition[] {
                A
                    .Class("Some.Namespace.Catalog")
                    .WithDescription("A set of items to show to customers.")
                    .Implementing("Some.Namespace.IAggregateRoot<CatalogId>"),
            });

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Ubiquitous language (Context\-specific domain terminology)

| Catalog                              |
| ------------------------------------ |
| A set of items to show to customers. |");
        }

        [Fact]
        public async Task Ubiquitous_language_ignores_abstract_classes()
        {
            _canvasSettings
                .UbiquitousLanguage
                .Returns(TypeDefinitionPredicates.From(
                    new ImplementsInterfaceMatching(".*IAggregateRoot<.*>"),
                    new WithModifiers(TypeModifiers.Concrete)
                ));

            Define(new TypeDefinition[] {
                A
                    .Class("Some.Namespace.Catalog")
                    .Abstract()
                    .Implementing("Some.Namespace.IAggregateRoot<CatalogId>"),
            });

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Ubiquitous language (Context\-specific domain terminology)

No ubiquitous language found
");
        }

        [Fact]
        public async Task No_inbound_communication_configuration_do_not_generate_inbound_communication_section()
        {
            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.MyCommand").Implementing("Some.Namespace.ICommand"),
                A.Class("Some.Namespace.MySecondCommand").Implementing("Some.Namespace.ICommand"),
            });

            var markdown = await GenerateMarkdown();

            markdown.Should().NotContain("## Inbound communication");
        }

        [Fact]
        public async Task No_inbound_communication_renders_not_found()
        {
            _canvasSettings
                .InboundCommunication
                .Returns(new InboundCommunication(
                    TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*ICommand")),
                    Enumerable.Empty<CollaboratorDefinition>(),
                    Enumerable.Empty<PolicyDefinition>())
                );

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Inbound communication

No inbound communication found
");
        }

        [Fact]
        public async Task Inbound_communication_renders_mermaid_graph()
        {
            _canvasSettings
                .InboundCommunication
                .Returns(new InboundCommunication(
                    TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*ICommand")),
                    new[] { new CollaboratorDefinition("WebApp", TypeDefinitionPredicates.From(new ImplementsInterfaceMatching("Some.Namespace.IController"))) },
                    new[] { new PolicyDefinition(new MethodAttributeMatch("Fact")) }
                ));

            var transactionController = A.Class("Web.TransactionController").Implementing("Some.Namespace.IController");
            var unitTests = A.Class("Tests.TransactionTests");
            
            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.RegisterNewTransactionCommand")
                    .Implementing("Some.Namespace.ICommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(transactionController)
                        .FromMethod(A.Method.Named("Register"))
                    )
                    .InstanciatedBy(An.Instanciator
                        .OfType(unitTests)
                        .FromMethod(A.Method
                            .Named("Transaction_registration_must_contain_a_not_paid_transaction")
                            .WithAttribute("Fact")
                        )
                    ),

                A.Class("Some.Namespace.RescheduleTransactionCommand")
                    .Implementing("Some.Namespace.ICommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(transactionController)
                        .FromMethod(A.Method.Named("Reschedule"))
                    )
                    .InstanciatedBy(An.Instanciator
                        .OfType(unitTests)
                        .FromMethod(A.Method
                            .Named("Transaction_reschedule_must_be_in_the_future")
                            .WithAttribute("Fact")
                        )
                    ),
            });

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Inbound communication

```mermaid
flowchart LR
    classDef collaborators fill:#FFE5FF;
    classDef policies fill:#FFFFAD, font-style:italic;
    SomeNamespaceRegisterNewTransactionCommand[""Register new transaction""]
    SomeNamespaceRegisterNewTransactionCommandWebAppCollaborator>""Web app""]
    class SomeNamespaceRegisterNewTransactionCommandWebAppCollaborator collaborators;
    SomeNamespaceRegisterNewTransactionCommandPolicies[/""Transaction registration must contain a not paid transaction""/]
    class SomeNamespaceRegisterNewTransactionCommandPolicies policies;
    SomeNamespaceRescheduleTransactionCommand[""Reschedule transaction""]
    SomeNamespaceRescheduleTransactionCommandWebAppCollaborator>""Web app""]
    class SomeNamespaceRescheduleTransactionCommandWebAppCollaborator collaborators;
    SomeNamespaceRescheduleTransactionCommandPolicies[/""Transaction reschedule must be in the future""/]
    class SomeNamespaceRescheduleTransactionCommandPolicies policies;
    SomeNamespaceRegisterNewTransactionCommandWebAppCollaborator --> SomeNamespaceRegisterNewTransactionCommand
    SomeNamespaceRegisterNewTransactionCommand --- SomeNamespaceRegisterNewTransactionCommandPolicies
    SomeNamespaceRescheduleTransactionCommandWebAppCollaborator --> SomeNamespaceRescheduleTransactionCommand
    SomeNamespaceRescheduleTransactionCommand --- SomeNamespaceRescheduleTransactionCommandPolicies
```");
        }

        // ----- Private

        private Task<string> GenerateMarkdown() => _generator.Generate(SomeSolution, SomeCanvasSettingsPath);

        private void Define(IEnumerable<TypeDefinition> types)
        {
            _typeDefinitionRepository
                .GetAll(SomeSolution)
                .Returns(Create(types));
        }

        private static async IAsyncEnumerable<TypeDefinition> Create(IEnumerable<TypeDefinition> elements)
        {
            await Task.Delay(0);

            foreach (var typeDefinition in elements) {
                yield return typeDefinition;
            }
        }
    }
}