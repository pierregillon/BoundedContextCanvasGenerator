using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Tests.Acceptance.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Command = BoundedContextCanvasGenerator.Domain.BC.Inbound.Command;

namespace BoundedContextCanvasGenerator.Tests.Acceptance
{
    public class RenderBoundedContextCanvasTests
    {
        private readonly RenderBoundedContextCanvas _exporter;

        public RenderBoundedContextCanvasTests()
        {
            var serviceProvider = new ServiceCollection()
                .RegisterApplication()
                .BuildServiceProvider();

            _exporter = serviceProvider.GetRequiredService<RenderBoundedContextCanvas>();
        }

        [Fact]
        public async Task Renders_default_name_when_not_defined()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas.Named(CanvasName.Default);

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().StartWith("# Bounded context canvas");
        }

        [Fact]
        public async Task Renders_canvas_name_as_h1()
        {
            const string name = "Catalog";

            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas.Named(new CanvasName(name));

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().StartWith($"# {name}");
        }

        [Fact]
        public async Task Do_not_render_definition_when_not_defined()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas;

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().NotContain("## Definition");
        }

        [Fact]
        public async Task Renders_description()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas
                .DefinedAs(A.CanvasDefinition.DescribedAs(new Text("A simple bounded context."))
            );

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().Contain(
@"### Description

> A simple bounded context.
");

        }

        [Fact]
        public async Task Renders_strategic_classification_as_a_table()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas
                .DefinedAs(A.CanvasDefinition.StrategicallyClassifiedAs(
                    new StrategicClassification(DomainType.CoreDomain, BusinessModel.RevenueGenerator, Evolution.Commodity)
                )
            );

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().Contain(
@"### Strategic classification [(?)](https://github.com/ddd-crew/bounded-context-canvas#strategic-classification)

| Domain                                         | Business Model                                         | Evolution                                             |
| ---------------------------------------------- | ------------------------------------------------------ | ----------------------------------------------------- |
| *Core domain*<br/>(a key strategic initiative) | *Revenue generator*<br/>(people pay directly for this) | *Commodity*<br/>(highly\-standardised versions exist) |
");
        }

        [Fact]
        public async Task Renders_domain_role_section()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas
                .DefinedAs(A.CanvasDefinition.WithRole(
                    new DomainRole(new Text("gateway context"), new Text("Provide catalog item allowing Basket, Ordering and Payment contexts to properly work."))
                )
            );

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().Contain(
@"### Domain role [(?)](https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md): *gateway context*

Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.
");
        }

        [Fact]
        public async Task Do_not_render_ubiquitous_language_if_not_defined()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas.WithUbiquitousLanguage(UbiquitousLanguage.None);

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().NotContain("## Ubiquitous language");
        }

        [Fact]
        public async Task Renders_core_concepts_with_its_description_as_table()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas
                .WithUbiquitousLanguage(new UbiquitousLanguage(new [] {
                    new CoreConcept("Catalog", "A set of items to show to customers.")
                }));

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().Contain(
@"## Ubiquitous language (Context\-specific domain terminology)

| Catalog                              |
| ------------------------------------ |
| A set of items to show to customers. |
");
        }

        [Fact]
        public async Task No_inbound_communication_configuration_do_not_generate_inbound_communication_section()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas
                .WithInboundCommunication(InboundCommunication.Empty);

            var markdown = await GenerateMarkdown(boundedContextCanvas);

            markdown.Should().NotContain("## Inbound communication");
        }

        [Fact]
        public async Task Inbound_communication_renders_mermaid_graph()
        {
            BoundedContextCanvas boundedContextCanvas = A.BoundedContextCanvas
                .WithInboundCommunication(A.InboundCommunication
                    .WithModule(A.DomainModule
                        .WithFlow(
                            A.DomainFlow
                                .WithCommand(new Command("Register new transaction", new TypeFullName("Some.Namespace.RegisterNewTransactionCommand")))
                                .WithCollaborator(new Collaborator("Web app"))
                                .WithPolicy(new Policy("Transaction registration must contain a not paid transaction")))
                        .WithFlow(
                            A.DomainFlow
                                .WithCommand(new Command("Reschedule transaction", new TypeFullName("Some.Namespace.RescheduleTransactionCommand")))
                                .WithCollaborator(new Collaborator("Web app"))
                                .WithPolicy(new Policy("Transaction reschedule must be in the future"))
                        ))
                );

            var markdown = await GenerateMarkdown(boundedContextCanvas);

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
```
");
        }

        // ----- Private

        private async Task<string> GenerateMarkdown(BoundedContextCanvas boundedContextCanvas) => (await _exporter.Export(boundedContextCanvas)).ToString();
    }
}