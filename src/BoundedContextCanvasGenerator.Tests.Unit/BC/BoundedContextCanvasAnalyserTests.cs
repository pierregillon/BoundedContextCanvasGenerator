using System;
using System.Collections.Generic;
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
    private readonly BoundedContextCanvasAnalyser analyser = new();

    public BoundedContextCanvasAnalyserTests()
    {
        _canvasSettings = Substitute.For<ICanvasSettings>();
        _canvasSettings.InboundCommunicationSettings.Returns(InboundCommunicationSettings.Empty);
    }

    [Fact]
    public async Task Link_command_and_domain_event_when_handler_of_a_command_instanciates_domain_event()
    {
        var canvas = await Analyse(
            new TypeDefinition[] {
                A.Class("OrderItemCommand")
            },
            new TypeDefinition[] {
                A.Class("ItemOrdered").InstanciatedBy(A.Class("OrderItemCommandHandler"))
            },
            Array.Empty<TypeDefinition>(),
            new[] {
                new LinkedTypeDefinition(
                    A.Class("OrderItemCommandHandler").Implementing("ICommandHandler<OrderItemCommand>"),
                    TypeDefinitionLink.From("T -> .*ICommandHandler<T>$")
                )
            }
        );

        var uniqueDomainFlow = canvas.InboundCommunication.Modules.Single().Flows.Single();

        uniqueDomainFlow
            .DomainEvents
            .Should()
            .BeEquivalentTo(new[] {
                new DomainEvent("Item ordered", new TypeFullName("ItemOrdered"), Enumerable.Empty<IntegrationEvent>())
            });
    }

    [Fact]
    public async Task Do_no_link_command_and_domain_event_when_handler_is_not_linked_to_the_command()
    {
        var canvas = await Analyse(
            new TypeDefinition[] {
                A.Class("OrderItemCommand")
            }, 
            new TypeDefinition[] {
                A.Class("ItemCreated").InstanciatedBy(A.Class("CreateItemCommandHandler"))
            },
            Array.Empty<TypeDefinition>(),
            new[] {
                new LinkedTypeDefinition(
                    A.Class("CreateItemCommandHandler").Implementing("ICommandHandler<CreateItemCommand>"),
                    TypeDefinitionLink.From("T -> .*ICommandHandler<T>$")
                )
            }
        );

        var uniqueDomainFlow = canvas.InboundCommunication.Modules.Single().Flows.Single();

        uniqueDomainFlow
            .DomainEvents
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task Link_event_and_integration_event()
    {
        var canvas = await Analyse(
            new TypeDefinition[] {
                A.Class("OrderItemCommand")
            }, 
            new TypeDefinition[] {
                A.Class("ItemRegistered").InstanciatedBy(A.Class("OrderItemCommandHandler"))
            },
            new TypeDefinition[] {
                A.Class("ItemCreated").InstanciatedBy(A.Class("PublishItemCreatedOnItemRegistered"))
            },
            new[] {
                new LinkedTypeDefinition(
                    A.Class("OrderItemCommandHandler").Implementing("ICommandHandler<OrderItemCommand>"),
                    TypeDefinitionLink.From("T -> .*ICommandHandler<T>$")
                ),
                new LinkedTypeDefinition(
                    A.Class("PublishItemCreatedOnItemRegistered").Implementing("IDomainEventListener<ItemRegistered>"),
                    TypeDefinitionLink.From("T -> .*IDomainEventListener<T>$")
                )
            }
        );

        var uniqueDomainFlow = canvas.InboundCommunication.Modules.Single().Flows.Single();

        uniqueDomainFlow
            .DomainEvents
            .Should()
            .BeEquivalentTo(new[] {
                new DomainEvent(
                    "Item registered", 
                    new TypeFullName("ItemRegistered"), 
                    new [] {
                        new IntegrationEvent("Item created", new TypeFullName("ItemCreated"))
                    }
                )
            });
    }

    [Fact]
    public async Task Do_no_link_domain_event_and_handler_when_not_handled()
    {
        var canvas = await Analyse(
            new TypeDefinition[] {
                A.Class("OrderItemCommand")
            }, 
            new TypeDefinition[] {
                A.Class("ItemRegistered").InstanciatedBy(A.Class("OrderItemCommandHandler")),
                A.Class("UserCreated")
            },
            new TypeDefinition[] {
                A.Class("ItemCreated").InstanciatedBy(A.Class("PublishItemCreatedOnItemRegistered"))
            },
            new[] {
                new LinkedTypeDefinition(
                    A.Class("OrderItemCommandHandler").Implementing("ICommandHandler<OrderItemCommand>"),
                    TypeDefinitionLink.From("T -> .*ICommandHandler<T>$")
                ),
                new LinkedTypeDefinition(
                    A.Class("PublishItemCreatedOnUserCreated").Implementing("IDomainEventListener<UserCreated>"),
                    TypeDefinitionLink.From("T -> .*IDomainEventListener<T>$")
                )
            }
        );

        var uniqueDomainFlow = canvas.InboundCommunication.Modules.Single().Flows.Single();

        uniqueDomainFlow
            .DomainEvents
            .Should()
            .BeEquivalentTo(new[] {
                new DomainEvent(
                    "Item registered", 
                    new TypeFullName("ItemRegistered"), 
                    new IntegrationEvent[]  { }
                )
            });
    }

    private async Task<BoundedContextCanvas> Analyse(
        IReadOnlyCollection<TypeDefinition> commands,
        IReadOnlyCollection<TypeDefinition> domainEvents,
        IReadOnlyCollection<TypeDefinition> integrationEvents,
        IReadOnlyCollection<LinkedTypeDefinition> commandHandlers
    )
    {
        ExtractedElements ToExtractedElements(IReadOnlyCollection<TypeDefinition> types)
        {
            return new ExtractedElements(types.Any(), types);
        }

        var typeExtract = new TypeDefinitionExtract(
            ToExtractedElements(commands),
            ToExtractedElements(domainEvents),
            ToExtractedElements(Array.Empty<TypeDefinition>()),
            commandHandlers,
            integrationEvents
        );

        return await analyser.Analyse(typeExtract, _canvasSettings);
    }
}