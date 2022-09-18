using System.Linq;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types;

public class TypeFullNameTests
{
    [Fact]
    public void Two_same_full_name_are_equals()
    {
        new TypeFullName("Test")
            .Should()
            .Be(new TypeFullName("Test"));
    }

    [Fact]
    public void Two_collections_are_sequencially_equals()
    {
        var first = new[] { new TypeFullName("Test2"), new TypeFullName("Test") };
        var second = new[] { new TypeFullName("Test2"), new TypeFullName("Test") };

        first.SequenceEqual(second).Should().BeTrue();
    }
}