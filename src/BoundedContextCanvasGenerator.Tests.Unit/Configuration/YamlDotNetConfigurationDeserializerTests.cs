using BoundedContextCanvasGenerator.Infrastructure.Configuration;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Configuration
{
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
    }
}
