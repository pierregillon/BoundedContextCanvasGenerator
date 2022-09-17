using System;
using System.Linq;
using System.Threading.Tasks;
using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.BC;

public class BoundedContextCanvasAnalyserTests
{
    private readonly ICanvasSettings _canvasSettings;

    public BoundedContextCanvasAnalyserTests()
    {
        _canvasSettings = Substitute.For<ICanvasSettings>();
        _canvasSettings.InboundCommunicationSettings.Returns(InboundCommunicationSettings.Empty);
    }

    [Fact]
    public async Task Link_command_and_domain_event_when_handler_of_a_command_instanciates_domain_event()
    {
        var analyser = new BoundedContextCanvasAnalyser();

        var typeExtract = new TypeDefinitionExtract(
            new ExtractedElements(true, new TypeDefinition[] {
                A.Class("OrderItemCommand")
            }),
            new ExtractedElements(true, new TypeDefinition[] {
                A.Class("ItemOrdered").InstanciatedBy(A.Class("OrderItemCommandHandler"))
            }),
            new ExtractedElements(false, Array.Empty<TypeDefinition>()),
            new[] {
                new LinkedTypeDefinition(
                    A.Class("OrderItemCommandHandler").Implementing("ICommandHandler<OrderItemCommand>"),
                    TypeDefinitionLink.From("T -> .*ICommandHandler<T>$")
                )
            }
        );

        var canvas = await analyser.Analyse(typeExtract, _canvasSettings);

        var uniqueDomainFlow = canvas.InboundCommunication.Modules.Single().Flows.Single();

        uniqueDomainFlow
            .DomainEvents
            .Should()
            .BeEquivalentTo(new[] {
                new DomainEvent("Item ordered", new TypeFullName("ItemOrdered"))
            });
    }

    [Fact]
    public async Task Do_no_link_command_and_domain_event_when_handler_is_not_linked_to_the_command()
    {
        var analyser = new BoundedContextCanvasAnalyser();

        var typeExtract = new TypeDefinitionExtract(
            new ExtractedElements(true, new TypeDefinition[] {
                A.Class("OrderItemCommand")
            }),
            new ExtractedElements(true, new TypeDefinition[] {
                A.Class("ItemCreated").InstanciatedBy(A.Class("CreateItemCommandHandler"))
            }),
            new ExtractedElements(false, Array.Empty<TypeDefinition>()),
            new[] {
                new LinkedTypeDefinition(
                    A.Class("CreateItemCommandHandler").Implementing("ICommandHandler<CreateItemCommand>"),
                    TypeDefinitionLink.From("T -> .*ICommandHandler<T>$")
                )
            }
        );

        var canvas = await analyser.Analyse(typeExtract, _canvasSettings);

        var uniqueDomainFlow = canvas.InboundCommunication.Modules.Single().Flows.Single();

        uniqueDomainFlow
            .DomainEvents
            .Should()
            .BeEmpty();
    }
}