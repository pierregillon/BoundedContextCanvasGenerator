using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Tests.Acceptance.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Acceptance;

public class GenerateBoundedContextCanvasFromSolutionPathTests
{
    private readonly GenerateBoundedContextCanvasFromSolutionPath _generator;
    private static readonly CanvasSettingsPath CanvasSettingsPath = new("test");
    private static readonly SolutionPath SolutionPath = new("test");
    private readonly ICanvasSettings _canvasSettings;
    private readonly ITypeDefinitionRepository _typeDefinitionRepository;

    public GenerateBoundedContextCanvasFromSolutionPathTests()
    {
        var canvasSettingsRepository = Substitute.For<ICanvasSettingsRepository>();

        _canvasSettings = Substitute.For<ICanvasSettings>();
        _canvasSettings.Name.Returns(CanvasName.Default);
        _canvasSettings.Definition.Returns(A.CanvasDefinition);
        _canvasSettings.UbiquitousLanguage.Returns(TypeDefinitionPredicates.Empty);
        _canvasSettings.InboundCommunicationSettings.Returns(InboundCommunicationSettings.Empty);
        _canvasSettings.InboundCommunicationSettings.Returns(InboundCommunicationSettings.Empty);

        canvasSettingsRepository
            .Get(CanvasSettingsPath)
            .Returns(_canvasSettings);

        _typeDefinitionRepository = Substitute.For<ITypeDefinitionRepository>();

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(Array.Empty<TypeDefinition>());

        var serviceProvider = new ServiceCollection()
            .RegisterApplication()
            .AddScoped(_ => canvasSettingsRepository)
            .AddScoped(_ => _typeDefinitionRepository)
            .BuildServiceProvider();

        _generator = serviceProvider.GetRequiredService<GenerateBoundedContextCanvasFromSolutionPath>();
    }

    [Fact]
    public async Task No_type_definition_generates_empty_bounded_context_canvas()
    {
        var boundedContextCanvas = await Generate();

        BoundedContextCanvas expected = A.BoundedContextCanvas;

        boundedContextCanvas
            .Should()
            .Be(expected);
    }

    [Fact]
    public async Task Name_and_definition_are_directly_read_from_settings()
    {
        _canvasSettings.Name.Returns(CanvasName.From("Catalog"));
        _canvasSettings.Definition.Returns(
            A.CanvasDefinition
                .DescribedAs(Text.From("Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated items."))
                .StrategicallyClassifiedAs(new StrategicClassification(DomainType.CoreDomain, BusinessModel.RevenueGenerator, Evolution.Commodity))
                .WithRole(new DomainRole(
                    new Text("gateway context"),
                    new Text("Provide catalog item allowing Basket, Ordering and Payment contexts to properly work."))
                )
        );

        var boundedContextCanvas = await Generate();

        BoundedContextCanvas expected = A.BoundedContextCanvas
            .Named(_canvasSettings.Name)
            .DefinedAs(_canvasSettings.Definition);

        boundedContextCanvas
            .Should()
            .Be(expected);
    }

    [Fact]
    public async Task Generates_ubiquitous_language_from_aggregates()
    {
        _canvasSettings
            .UbiquitousLanguage
            .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching("IAggregateRoot")));

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(CollectionFrom(
                A.Class("Catalog")
                    .WithDescription("This is a simple product catalog")
                    .Implementing("IAggregateRoot"),
                A.Class("CatalogItem")
                    .WithDescription("Item of a catalog")
                    .Implementing("IAggregateRoot")
            ));

        var boundedContextCanvas = await Generate();

        boundedContextCanvas
            .UbiquitousLanguage
            .Should()
            .Be(An.UbiquitousLanguage
                .WithCoreConcept(new CoreConcept("Catalog", "This is a simple product catalog"))
                .WithCoreConcept(new CoreConcept("Catalog item", "Item of a catalog"))
                .Build());
    }

    [Fact]
    public async Task Generates_a_domain_flow_from_command()
    {
        _canvasSettings
            .InboundCommunicationSettings
            .Returns(An.InboundCommunicationSettings
                .WithCommandMatching(TypeDefinitionPredicates.From(new NamedLike(".*Command$")))
            );

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(CollectionFrom(
                A.Class("Some.Namespace.RegisterCatalogCommand")
            ));

        var boundedContextCanvas = await Generate();

        boundedContextCanvas
            .InboundCommunication
            .Modules
            .Should()
            .BeEquivalentTo(new DomainModule[] {
                A.DomainModule
                    .Named("Namespace")
                    .WithFlow(
                        A.DomainFlow
                            .WithCommand(new Command("Register catalog", new TypeFullName("Some.Namespace.RegisterCatalogCommand")))
                    )
            });
    }

    [Fact]
    public async Task Do_not_generates_collaborators_when_no_settings_defined()
    {
        _canvasSettings
            .InboundCommunicationSettings
            .Returns(An.InboundCommunicationSettings
                .WithCommandMatching(TypeDefinitionPredicates.From(new NamedLike(".*Command$")))
            );

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(CollectionFrom(
                A.Class("Some.Namespace.RegisterCatalogCommand")
                    .InstanciatedBy(A.Class("Some.Namespace.CatalogController"))
            ));

        var boundedContextCanvas = await Generate();

        boundedContextCanvas
            .InboundCommunication
            .Modules
            .Should()
            .BeEquivalentTo(new DomainModule[] {
                A.DomainModule
                    .Named("Namespace")
                    .WithFlow(
                        A.DomainFlow
                            .WithCommand(new Command("Register catalog", new TypeFullName("Some.Namespace.RegisterCatalogCommand")))
                    )
            });
    }

    [Fact]
    public async Task Generates_collaborator_from_command_instanciator()
    {
        _canvasSettings
            .InboundCommunicationSettings
            .Returns(An.InboundCommunicationSettings
                .WithCommandMatching(TypeDefinitionPredicates.From(new NamedLike(".*Command$")))
                .WithCollaboratorDefinition(new CollaboratorDefinition("Web app", TypeDefinitionPredicates.From(new NamedLike(".*Controller$"))))
            );

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(CollectionFrom(
                A.Class("Some.Namespace.RegisterCatalogCommand")
                    .InstanciatedBy(A.Class("Some.Namespace.CatalogController"))
            ));

        var boundedContextCanvas = await Generate();

        boundedContextCanvas
            .InboundCommunication
            .Modules
            .Should()
            .BeEquivalentTo(new DomainModule[] {
                A.DomainModule
                    .Named("Namespace")
                    .WithFlow(
                        A.DomainFlow
                            .WithCommand(new Command("Register catalog", new TypeFullName("Some.Namespace.RegisterCatalogCommand")))
                            .WithCollaborator(new Collaborator("Web app"))
                    )
            });
    }

    [Fact]
    public async Task Generates_policies_from_command_instanciators()
    {
        _canvasSettings
            .InboundCommunicationSettings
            .Returns(An.InboundCommunicationSettings
                .WithCommandMatching(TypeDefinitionPredicates.From(new NamedLike(".*Command$")))
                .WithPolicyDefinition(new PolicyDefinition(new MethodAttributeMatch("Fact"))
            ));

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(CollectionFrom(
                A.Class("Some.Namespace.RegisterCatalogCommand")
                    .InstanciatedBy(
                        A.Class("Some.Namespace.RegisterCatalogCommandTests"), 
                        A.Method.Named("A_catalog_must_have_at_least_one_item").WithAttribute("Fact"),
                        A.Method.Named("A_catalog_must_have_a_non_empty_name").WithAttribute("Fact")
                    ),
                A.Class("Some.Namespace.RegisterCatalogCommandTests")
            ));

        var boundedContextCanvas = await Generate();

        boundedContextCanvas
            .InboundCommunication
            .Modules
            .Should()
            .BeEquivalentTo(new DomainModule[] {
                A.DomainModule
                    .Named("Namespace")
                    .WithFlow(
                        A.DomainFlow
                            .WithCommand(new Command("Register catalog", new TypeFullName("Some.Namespace.RegisterCatalogCommand")))
                            .WithPolicy(new Policy("A catalog must have at least one item"))
                            .WithPolicy(new Policy("A catalog must have a non empty name"))
                    )
            });
    }

    [Fact]
    public async Task Generates_inbound_communication_modules_from_command_namespace()
    {
        _canvasSettings
            .InboundCommunicationSettings
            .Returns(An.InboundCommunicationSettings
                .WithCommandMatching(TypeDefinitionPredicates.From(new NamedLike(".*Command$")))
            );

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(CollectionFrom(
                A.Class("Some.Namespace.Catalog.RegisterCatalogCommand"),
                A.Class("Some.Namespace.Contact.AddContactCommand")
            ));

        var boundedContextCanvas = await Generate();

        boundedContextCanvas
            .InboundCommunication
            .Modules
            .Should()
            .BeEquivalentTo(new DomainModule[] {
                A.DomainModule
                    .Named("Catalog")
                    .WithFlow(
                        A.DomainFlow
                            .WithCommand(new Command("Register catalog", new TypeFullName("Some.Namespace.Catalog.RegisterCatalogCommand")))
                    ),
                A.DomainModule
                    .Named("Contact")
                    .WithFlow(
                        A.DomainFlow.WithCommand(new Command("Add contact", new TypeFullName("Some.Namespace.Contact.AddContactCommand")))
                    )
            });
    }

    [Fact]
    public async Task Generates_domain_event_instanciated_by_command()
    {
        _canvasSettings
            .InboundCommunicationSettings
            .Returns(An.InboundCommunicationSettings
                .WithCommandMatching(TypeDefinitionPredicates.From(new NamedLike(".*Command$")))
                .WithDomainEventDefinition(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching("IDomainEvent")))
            );

        _typeDefinitionRepository
            .GetAll(SolutionPath)
            .Returns(CollectionFrom(
                A.Class("Some.Namespace.Catalog.RegisterCatalogCommand"),
                A.Class("Some.Namespace.Catalog.CatalogRegistered")
                    .Implementing("IDomainEvent")
                    .InstanciatedBy(A.Class("Some.Namespace.Catalog.RegisterCatalogCommand"))
            ));

        var boundedContextCanvas = await Generate();

        boundedContextCanvas
            .InboundCommunication
            .Modules
            .Should()
            .BeEquivalentTo(new DomainModule[] {
                A.DomainModule
                    .Named("Catalog")
                    .WithFlow(
                        A.DomainFlow
                            .WithCommand(new Command("Register catalog", new TypeFullName("Some.Namespace.Catalog.RegisterCatalogCommand")))
                            .WithDomainEvent(new DomainEvent("Catalog registered", new TypeFullName("Some.Namespace.Catalog.CatalogRegistered")))
                    )
            });
    }

    private Task<BoundedContextCanvas> Generate() => _generator.Generate(SolutionPath, CanvasSettingsPath);
    public static IReadOnlyCollection<TypeDefinition> CollectionFrom(params TypeDefinition[] typeDefinitions) => typeDefinitions;
}