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
        public void Deserializes_commands()
        {
            const string yaml = 
@"commands:
    type: 'class'
    implementing:
        pattern: '.*ICommand$'";

            var configuration = _deserializer.Deserialize(yaml);

            configuration.Commands!.Type.Should().Be("class");
            configuration.Commands!.Implementing!.Pattern.Should().Be(".*ICommand$");
        }
        
        [Fact]
        public void Deserializes_domain_events()
        {
            const string yaml =
@"domainEvents:
    type: 'class'
    implementing:
        pattern: '.*IDomainEvent$'";

            var configuration = _deserializer.Deserialize(yaml);

            configuration.DomainEvents!.Type.Should().Be("class");
            configuration.DomainEvents!.Implementing!.Pattern.Should().Be(".*IDomainEvent$");
        }
    }
}
