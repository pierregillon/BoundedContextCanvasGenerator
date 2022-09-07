using System;
using System.Collections.Generic;
using System.Linq;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Markdown;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
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

    public class InboundCommunicationFlowChartBuilderTests2
    {
        [Fact]
        public void Cannot_generate_flowchart_from_empty_collection()
        {
            Action action = () => _ = new InboundCommunicationFlowChartBuilder2(
                Enumerable.Empty<CollaboratorDefinition2>(), 
                Enumerable.Empty<PolicyDefinition>()
            ).Build(Array.Empty<TypeDefinition>());

            action
                .Should()
                .Throw<ArgumentException>();
        }

        [Fact]
        public void Generate_flowchart_with_simple_command()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
            };

            GenerateMermaid(types)
                .Should()
                .Be(
@"```mermaid
flowchart LR
    TestNamespaceOrderNewProductCommand[""Order new product""]
```");
        }

        [Fact]
        public void A_non_configured_collaborator_ignored()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController"))
                        .FromMethod(A.Method.Named("Post"))
                    )
            };

            GenerateMermaid(types)
                .Should()
                .Be(
@"```mermaid
flowchart LR
    TestNamespaceOrderNewProductCommand[""Order new product""]
```");
        }

        [Fact]
        public void A_collaborator_is_a_class_instanciating_a_command()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController"))
                        .FromMethod(A.Method.Named("Post"))
                    )
            };

            GenerateMermaid(types, new[] {
                    new CollaboratorDefinition2("WebApp", TypeDefinitionPredicates.From(new NamedLike(".*Controller$")))
                })
                .Should()
                .Be(
                    @"```mermaid
flowchart LR
    classDef collaborators fill:#FFE5FF;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandWebAppCollaborator>""Web app""]
    class TestNamespaceOrderNewProductCommandWebAppCollaborator collaborators;
    TestNamespaceOrderNewProductCommandWebAppCollaborator --> TestNamespaceOrderNewProductCommand
```");
        }

        [Fact]
        public void Two_collaborators_of_the_same_command_are_linked_with_the_command()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController"))
                        .FromMethod(A.Method.Named("Post"))
                    )
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("MobileController"))
                        .FromMethod(A.Method.Named("Post"))
                    )
            };

            GenerateMermaid(types, new [] { 
                new CollaboratorDefinition2("WebApp", TypeDefinitionPredicates.From(new NamedLike("UserController"))),
                new CollaboratorDefinition2("MobileApp", TypeDefinitionPredicates.From(new NamedLike("MobileController"))),
            })
                .Should()
                .Be(
@"```mermaid
flowchart LR
    classDef collaborators fill:#FFE5FF;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandWebAppCollaborator>""Web app""]
    class TestNamespaceOrderNewProductCommandWebAppCollaborator collaborators;
    TestNamespaceOrderNewProductCommandMobileAppCollaborator>""Mobile app""]
    class TestNamespaceOrderNewProductCommandMobileAppCollaborator collaborators;
    TestNamespaceOrderNewProductCommandWebAppCollaborator --> TestNamespaceOrderNewProductCommand
    TestNamespaceOrderNewProductCommandMobileAppCollaborator --> TestNamespaceOrderNewProductCommand
```");
        }

        [Fact]
        public void The_same_collaborator_is_duplicated_for_each_commands_in_order_to_have_an_horizontal_flow()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController"))
                        .FromMethod(A.Method.Named("OrderNewProduct"))
                    ),

                A.Class("Test.Namespace.CancelOrderCommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController"))
                        .FromMethod(A.Method.Named("CancelOrder"))
                    )
            };

            var mermaid = GenerateMermaid(types, new [] { 
                new CollaboratorDefinition2("WebApp", TypeDefinitionPredicates.From(new NamedLike(".*Controller$")))
            });

            mermaid
                .Should()
                .Be(
@"```mermaid
flowchart LR
    classDef collaborators fill:#FFE5FF;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandWebAppCollaborator>""Web app""]
    class TestNamespaceOrderNewProductCommandWebAppCollaborator collaborators;
    TestNamespaceCancelOrderCommand[""Cancel order""]
    TestNamespaceCancelOrderCommandWebAppCollaborator>""Web app""]
    class TestNamespaceCancelOrderCommandWebAppCollaborator collaborators;
    TestNamespaceOrderNewProductCommandWebAppCollaborator --> TestNamespaceOrderNewProductCommand
    TestNamespaceCancelOrderCommandWebAppCollaborator --> TestNamespaceCancelOrderCommand
```");
        }

        [Fact]
        public void A_command_policy_is_a_test_method_name()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
                    .InstanciatedBy(A.Class("Tests.OrderNewProductCommandTests"), new MethodInfo(
                        new MethodName("Must_contains_at_least_one_item_to_order"), new [] {
                            new MethodAttribute("Fact"),
                            new MethodAttribute("Trait(\"Category\", \"BoundedContextCanvasPolicy\")")
                        }
                    )),
            };

            var mermaid = GenerateMermaid(
                types,
                Array.Empty<CollaboratorDefinition2>(),
                new PolicyDefinition[] { new(new MethodAttributeMatch("Trait\\(\"Category\", \"BoundedContextCanvasPolicy\"\\)")) }
            );

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
        public void Command_policies_are_separated_with_html_new_line_tag()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
                    .InstanciatedBy(A.Class("Tests.OrderNewProductCommandTests"), 
                        new MethodInfo(
                            new MethodName("Must_contains_at_least_one_item_to_order"), new[] {
                                new MethodAttribute("Fact"),
                                new MethodAttribute("Trait(\"Category\", \"BoundedContextCanvasPolicy\")")
                            }
                        ), new MethodInfo(
                            new MethodName("Cannot_be_altered"), new[] {
                                new MethodAttribute("Fact"),
                                new MethodAttribute("Trait(\"Category\", \"BoundedContextCanvasPolicy\")")
                            })
                        ),
            };

            var mermaid = GenerateMermaid(
                types,
                Array.Empty<CollaboratorDefinition2>(),
                new PolicyDefinition[] { new(new MethodAttributeMatch("Trait\\(\"Category\", \"BoundedContextCanvasPolicy\"\\)")) }
            );

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

            var mermaid = GenerateMermaid(types);

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
        public void Two_different_collaborators_instanciating_the_same_command_create_a_single_link()
        {
            var types = new TypeDefinition[] {
                A.Class("Test.Namespace.OrderNewProductCommand")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController"))
                        .FromMethod(A.Method.Named("OrderNewProduct"))
                    )
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("AdminController"))
                        .FromMethod(A.Method.Named("OrderNewProduct"))
                    ),
            };

            var mermaid = GenerateMermaid(types, new[] {
                new CollaboratorDefinition2("WebApp", TypeDefinitionPredicates.From(new NamedLike(".*Controller$")))
            });

            mermaid
                .Should()
                .Be(
                    @"```mermaid
flowchart LR
    classDef collaborators fill:#FFE5FF;
    TestNamespaceOrderNewProductCommand[""Order new product""]
    TestNamespaceOrderNewProductCommandWebAppCollaborator>""Web app""]
    class TestNamespaceOrderNewProductCommandWebAppCollaborator collaborators;
    TestNamespaceOrderNewProductCommandWebAppCollaborator --> TestNamespaceOrderNewProductCommand
```");
        }

        private static string GenerateMermaid(TypeDefinition[] types, IEnumerable<CollaboratorDefinition2>? collaborators = null, IEnumerable<PolicyDefinition>? policyDefinitions = null)
        {
            var builder = new InboundCommunicationFlowChartBuilder2(
                collaborators ?? Enumerable.Empty<CollaboratorDefinition2>(),
                policyDefinitions ?? Enumerable.Empty<PolicyDefinition>()
            );

            return builder.Build(types).ToString().Trim('\r', '\n');
        }
    }
}