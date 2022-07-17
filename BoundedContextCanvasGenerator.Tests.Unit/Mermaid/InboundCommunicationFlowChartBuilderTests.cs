using System;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;
using FluentAssertions;
using Xunit;

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
                TypeDefinitionBuilder.Class("Test.Namespace.OrderNewProductCommand")
            };

            GenerateMermaid(types)
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

//            GenerateMermaid(types)
//                .Should()
//                .Be(
//@"```mermaid
//flowchart LR
//    Collaborators>""WebApp""]
//    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
//    TestNamespaceOrderNewProductCommand[""Order new product""]
//    Collaborators --> TestNamespaceOrderNewProductCommand
//```");
        }

        [Fact]
        public void Generate_flowchart_with_commands_in_different_folders()
        {
            var types = new TypeDefinition[] {
                TypeDefinitionBuilder.Class("Test.Namespace.Order.OrderNewProductCommand"),
                TypeDefinitionBuilder.Class("Test.Namespace.Contact.EditContactDetailsCommand"),
            };

            GenerateMermaid(types)
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
            
//            GenerateMermaid(types)
//                .Should()
//                .Be(
//@"### Order

//------------

//```mermaid
//flowchart LR
//    Collaborators>""WebApp""]
//    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
//    TestNamespaceOrderOrderNewProductCommand[""Order new product""]
//    Collaborators --> TestNamespaceOrderOrderNewProductCommand
//```

//### Contact

//------------

//```mermaid
//flowchart LR
//    Collaborators>""WebApp""]
//    style Collaborators fill:#f9f,stroke:#333,stroke-width:2px
//    TestNamespaceContactEditContactDetailsCommand[""Edit contact details""]
//    Collaborators --> TestNamespaceContactEditContactDetailsCommand
//```");
        }

        private static string GenerateMermaid(params TypeDefinition[] types)
        {
            var builder = InboundCommunicationFlowChartBuilder.From(types);

            return builder.Build().ToString().Trim('\r', '\n');
        }
    }
}