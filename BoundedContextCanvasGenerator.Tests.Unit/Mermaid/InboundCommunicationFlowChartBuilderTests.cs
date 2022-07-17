using System;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;
using FluentAssertions;
using Xunit;

using A = BoundedContextCanvasGenerator.Tests.Unit.TypeDefinitionBuilder;

namespace BoundedContextCanvasGenerator.Tests.Unit.Mermaid
{
    public class InboundCommunicationFlowChartBuilderTests
    {
        [Fact]
        public void Cannot_generate_flowchart_from_empty_collection()
        {
            Action action = () => _ = InboundCommunicationFlowChartBuilder.From(Array.Empty<TypeDefinition>());

            action
                .Should()
                .Throw<ArgumentException>();
        }

        [Fact]
        public void Generate_flowchart_with_single_command()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
            };

            GenerateMermaid(types, false)
                .Should()
                .Be(
@"```mermaid
flowchart LR
    Collaborators>""WebApp""]
    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
    Test
    TestNamespace[""Namespace""]
    TestNamespaceOrderNewProductCommand[""Order new product""]
    Collaborators --> Test
    Test --> TestNamespace
    TestNamespace --> TestNamespaceOrderNewProductCommand
```");
        }

        [Fact]
        public void Generate_flowchart_with_commands_in_the_same_folder()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.Order.OrderNewProductCommand"),
                A.Class("Test.Namespace.Order.CancelOrderCommand"),
            };

            GenerateMermaid(types, false)
                .Should()
                .Be(
                    @"```mermaid
flowchart LR
    Collaborators>""WebApp""]
    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
    Test
    TestNamespace[""Namespace""]
    TestNamespaceOrder[""Order""]
    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderCancelOrderCommand[""Cancel order""]
    Collaborators --> Test
    Test --> TestNamespace
    TestNamespace --> TestNamespaceOrder
    TestNamespaceOrder --> TestNamespaceOrderOrderNewProductCommand
    TestNamespaceOrder --> TestNamespaceOrderCancelOrderCommand
```");
        }

        [Fact]
        public void Generate_flowchart_with_commands_in_different_folders()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.Order.OrderNewProductCommand"),
                A.Class("Test.Namespace.Contact.EditContactDetailsCommand"),
            };

            GenerateMermaid(types, false)
                .Should()
                .Be(
@"```mermaid
flowchart LR
    Collaborators>""WebApp""]
    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
    Test
    TestNamespace[""Namespace""]
    TestNamespaceOrder[""Order""]
    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
    TestNamespaceContact[""Contact""]
    TestNamespaceContactEditContactDetailsCommand[""Edit contact details""]
    Collaborators --> Test
    Test --> TestNamespace
    TestNamespace --> TestNamespaceOrder
    TestNamespaceOrder --> TestNamespaceOrderOrderNewProductCommand
    TestNamespace --> TestNamespaceContact
    TestNamespaceContact --> TestNamespaceContactEditContactDetailsCommand
```");
        }

        [Fact]
        public void Does_not_generate_folders_from_contained_assembly_namespace()
        {
            var types = new TypeDefinition[] {
                A
                    .Class("Test.Namespace.Order.OrderNewProductCommand")
                    .InAssembly("Test.Namespace"),
                A
                    .Class("Test.Namespace.Contact.EditContactDetailsCommand")
                    .InAssembly("Test.Namespace"),
            };

            GenerateMermaid(types)
                .Should()
                .Be(
@"```mermaid
flowchart LR
    Collaborators>""WebApp""]
    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
    TestNamespaceOrder[""Order""]
    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
    TestNamespaceContact[""Contact""]
    TestNamespaceContactEditContactDetailsCommand[""Edit contact details""]
    Collaborators --> TestNamespaceOrder
    TestNamespaceOrder --> TestNamespaceOrderOrderNewProductCommand
    Collaborators --> TestNamespaceContact
    TestNamespaceContact --> TestNamespaceContactEditContactDetailsCommand
```");
        }

        [Fact]
        public void Split_into_lanes()
        {
            var types = new TypeDefinition[] {
                A
                    .Class("Test.Namespace.Order.OrderNewProductCommand")
                    .InAssembly("Test.Namespace"),
                A
                    .Class("Test.Namespace.Contact.EditContactDetailsCommand")
                    .InAssembly("Test.Namespace"),
            };

            GenerateMermaid(types, true)
                .Should()
                .Be(
@"### Order

---

```mermaid
flowchart LR
    Collaborators>""WebApp""]
    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
    Collaborators --> TestNamespaceOrderOrderNewProductCommand
```

### Contact

---

```mermaid
flowchart LR
    Collaborators>""WebApp""]
    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
    TestNamespaceContactEditContactDetailsCommand[""Edit contact details""]
    Collaborators --> TestNamespaceContactEditContactDetailsCommand
```");
        }

        private static string GenerateMermaid(TypeDefinition[] types, bool splitIntoLanes = false)
        {
            var builder = InboundCommunicationFlowChartBuilder.From(types);

            return builder.Build(splitIntoLanes).ToString().Trim('\r', '\n');
        }
    }
}