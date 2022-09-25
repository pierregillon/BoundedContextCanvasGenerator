using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.BC;

public class IntegrationEventTests
{
    [Fact]
    public void Creating_domain_event_from_type_remove_domain_even_keyword_suffix()
    {
        var integrationEvent = IntegrationEvent.FromType(A.Class("UserCreatedIntegrationEvent"));

        integrationEvent.FriendlyName.Should().Be("User created");
    }
}