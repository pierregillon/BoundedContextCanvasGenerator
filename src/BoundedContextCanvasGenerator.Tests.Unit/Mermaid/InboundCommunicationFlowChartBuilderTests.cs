using System;
using System.Linq;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;
using Command = BoundedContextCanvasGenerator.Domain.BC.Inbound.Command;

namespace BoundedContextCanvasGenerator.Tests.Unit.Mermaid;

public class InboundCommunicationFlowChartBuilderTests
{
    [Fact]
    public void Cannot_generate_flowchart_from_empty_inbound_communication()
    {
        Action action = () => _ = new InboundCommunicationFlowChartBuilder(InboundCommunication.Empty);

        action
            .Should()
            .Throw<ArgumentNullException>();
    }

    [Fact]
    public void Renders_domain_flow_with_only_a_command()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.OrderNewProductCommand")))
                )
            );

        GenerateMermaid(inboundCommunication)
            .Should()
            .Be(
@"```mermaid
flowchart LR
    TestNamespaceOrderNewProductCommand[""Order new product""]
```");
    }

    [Fact]
    public void Renders_front_as_collaborator()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.OrderNewProductCommand")))
                    .WithCollaborator(new Collaborator("Web app", CollaboratorType.Front))
                )
            );

        GenerateMermaid(inboundCommunication)
            .Should()
            .Be(
@"```mermaid
flowchart LR
    classDef frontCollaborators fill:#FFE5FF;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandWebAppCollaborator>""Web app""]
    class TestNamespaceOrderNewProductCommandWebAppCollaborator frontCollaborators;
    TestNamespaceOrderNewProductCommandWebAppCollaborator --> TestNamespaceOrderNewProductCommand
```");

    }
    
    [Fact]
    public void Renders_other_bounded_context_as_collaborator()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.OrderNewProductCommand")))
                    .WithCollaborator(new Collaborator("Web app", CollaboratorType.BoundedContext))
                )
            );

        GenerateMermaid(inboundCommunication)
            .Should()
            .Be(
@"```mermaid
flowchart LR
    classDef boundedContextCollaborators fill:#FF5C5C;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandWebAppCollaborator>""Web app""]
    class TestNamespaceOrderNewProductCommandWebAppCollaborator boundedContextCollaborators;
    TestNamespaceOrderNewProductCommandWebAppCollaborator --> TestNamespaceOrderNewProductCommand
```");
    }

    [Fact]
    public void Renders_two_different_collaborators_of_a_single_command()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.OrderNewProductCommand")))
                    .WithCollaborator(new Collaborator("Web app", CollaboratorType.Front))
                    .WithCollaborator(new Collaborator("Mobile app", CollaboratorType.Front))
                )
            );

        GenerateMermaid(inboundCommunication)
            .Should()
            .Be(
                @"```mermaid
flowchart LR
    classDef frontCollaborators fill:#FFE5FF;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandWebAppCollaborator>""Web app""]
    class TestNamespaceOrderNewProductCommandWebAppCollaborator frontCollaborators;
    TestNamespaceOrderNewProductCommandMobileAppCollaborator>""Mobile app""]
    class TestNamespaceOrderNewProductCommandMobileAppCollaborator frontCollaborators;
    TestNamespaceOrderNewProductCommandWebAppCollaborator --> TestNamespaceOrderNewProductCommand
    TestNamespaceOrderNewProductCommandMobileAppCollaborator --> TestNamespaceOrderNewProductCommand
```");
    }

    [Fact]
    public void Renders_command_policy()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.OrderNewProductCommand")))
                    .WithPolicy(new Policy("Must contains at least one item to order"))
                )
            );

        var mermaid = GenerateMermaid(inboundCommunication);

        mermaid
            .Should()
            .Be(
                @"```mermaid
flowchart LR
    classDef policies fill:#FFFFAD, font-style:italic;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandPolicies[/""Must contains at least one item to order""/]
    class TestNamespaceOrderNewProductCommandPolicies policies;
    TestNamespaceOrderNewProductCommand --- TestNamespaceOrderNewProductCommandPolicies
```");
    }

    [Fact]
    public void Rendering_multiple_command_policies_separates_them_with_html_new_line_tag()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.OrderNewProductCommand")))
                    .WithPolicy(new Policy("Must contains at least one item to order"))
                    .WithPolicy(new Policy("Cannot be altered"))
                )
            );

        var mermaid = GenerateMermaid(inboundCommunication);

        mermaid
            .Should()
            .Be(
                @"```mermaid
flowchart LR
    classDef policies fill:#FFFFAD, font-style:italic;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandPolicies[/""Must contains at least one item to order<br/>Cannot be altered""/]
    class TestNamespaceOrderNewProductCommandPolicies policies;
    TestNamespaceOrderNewProductCommand --- TestNamespaceOrderNewProductCommandPolicies
```");
    }

    [Fact]
    public void Rendering_multiple_domain_modules_separate_them_with_name_and_separator()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .Named("Order")
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.Order.OrderNewProductCommand")))
                )
            ).WithModule(A.DomainModule
                .Named("Contact")
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Edit contact details", new TypeFullName("Test.Namespace.Contact.EditContactDetailsCommand")))
                )
            );

        var mermaid = GenerateMermaid(inboundCommunication);

        mermaid
            .Should()
            .Be(
@"### Order

---

```mermaid
flowchart LR
    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
```

### Contact

---

```mermaid
flowchart LR
    TestNamespaceContactEditContactDetailsCommand[""Edit contact details""]
```");
    }

    [Fact]
    public void Renders_command_and_domain_event()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .Named("Order")
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.Order.OrderNewProductCommand")))
                    .WithDomainEvent(new DomainEvent("Product ordered", new TypeFullName("Test.Namespace.Order.ProductOrdered"), Enumerable.Empty<IntegrationEvent>()))
                )
            );

        var mermaid = GenerateMermaid(inboundCommunication);

        mermaid
            .Should()
            .Be(
@"```mermaid
flowchart LR
    classDef domainEvents fill:#FFA431;
    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderProductOrdered[""Product ordered""]
    class TestNamespaceOrderProductOrdered domainEvents;
    TestNamespaceOrderOrderNewProductCommand -.-> TestNamespaceOrderProductOrdered
```");
    }

    [Fact]
    public void Renders_command_policy_and_domain_event()
    {
        InboundCommunication inboundCommunication = A.InboundCommunication
            .WithModule(A.DomainModule
                .Named("Order")
                .WithFlow(A.DomainFlow
                    .WithCommand(new Command("Order new product", new TypeFullName("Test.Namespace.Order.OrderNewProductCommand")))
                    .WithPolicy(new Policy("Must contains at least one item to order"))
                    .WithDomainEvent(new DomainEvent("Product ordered", new TypeFullName("Test.Namespace.Order.ProductOrdered"), Enumerable.Empty<IntegrationEvent>()))
                )
            );

        var mermaid = GenerateMermaid(inboundCommunication);

        mermaid
            .Should()
            .Be(
@"```mermaid
flowchart LR
    classDef policies fill:#FFFFAD, font-style:italic;
    classDef domainEvents fill:#FFA431;
    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderOrderNewProductCommandPolicies[/""Must contains at least one item to order""/]
    class TestNamespaceOrderOrderNewProductCommandPolicies policies;
    TestNamespaceOrderProductOrdered[""Product ordered""]
    class TestNamespaceOrderProductOrdered domainEvents;
    TestNamespaceOrderOrderNewProductCommand --- TestNamespaceOrderOrderNewProductCommandPolicies
    TestNamespaceOrderOrderNewProductCommandPolicies -.-> TestNamespaceOrderProductOrdered
```");
    }

    private static string GenerateMermaid(InboundCommunication inboundCommunication)
    {
        var builder = new InboundCommunicationFlowChartBuilder(inboundCommunication);

        return builder.Build().ToString().Trim('\r', '\n');
    }
}