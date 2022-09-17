using BoundedContextCanvasGenerator.Infrastructure.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Configuration;

public class YamlDotNetConfigurationDeserializerTests
{
    private readonly YamlDotNetConfigurationDeserializer _deserializer;

    public YamlDotNetConfigurationDeserializerTests() => _deserializer = new YamlDotNetConfigurationDeserializer();

    [Fact]
    public void Deserializes_definition()
    {
        const string yaml =
            @"definition:
    description: 'Display the product catalog and the items available to purchase.'
    strategic_classification:
        domain: 'core'
        business_model: 'revenue_generator'
        evolution: 'commodity'
    domain_role:
        name: 'gateway context'
        description: 'Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.'
    ";

        var configuration = _deserializer.Deserialize(yaml);

        configuration.Definition!.Description!.Should().Be("Display the product catalog and the items available to purchase.");
        configuration.Definition!.StrategicClassification!.Domain.Should().Be("core");
        configuration.Definition!.StrategicClassification!.BusinessModel.Should().Be("revenue_generator");
        configuration.Definition!.StrategicClassification!.Evolution.Should().Be("commodity");
        configuration.Definition!.DomainRole!.Name.Should().Be("gateway context");
        configuration.Definition!.DomainRole!.Description.Should().Be("Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.");
    }

    [Fact]
    public void Deserializes_inbound_communication()
    {
        const string yaml =
            @"inbound_communication:
    commands: 
        selector: class implementing '.*ICommand$'
        handler: 
            selector: class implementing '.*ICommandHandler<.*>$'
            link: T -> .*ICommandHandler<T>$
    collaborators:
        - name: WebApp
          selector: class named '.*Controller$'
    policies:
        - method_attribute_pattern: 'Fact'
    domain_events:
        selector: class implementing 'IDomainEvent'
    ";

        var configuration = _deserializer.Deserialize(yaml);

        configuration.InboundCommunication!.Commands!
            .Should()
            .Be(new CommandDefinitionDto {
                Selector = "class implementing '.*ICommand$'",
                Handler = new HandlerDefinitionDto {
                    Selector = "class implementing '.*ICommandHandler<.*>$'",
                    Link = "T -> .*ICommandHandler<T>$"
                }
            });

        configuration.InboundCommunication.Collaborators!
            .Should()
            .BeEquivalentTo(new[] {
                new CollaboratorDto { Name = "WebApp", Selector = "class named '.*Controller$'" }
            });

        configuration.InboundCommunication.Policies!
            .Should()
            .BeEquivalentTo(new[] {
                new PolicyDto { MethodAttributePattern = "Fact" }
            });

        configuration.InboundCommunication.DomainEvents!
            .Should()
            .Be(new DomainEventDefinitionDto {
                Selector = "class implementing 'IDomainEvent'"
            });
    }
}