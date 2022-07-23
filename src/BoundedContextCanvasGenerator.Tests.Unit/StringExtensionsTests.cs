using BoundedContextCanvasGenerator.Domain;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit;

public class StringExtensionsTests
{
    [Fact]
    public void Snake_case_to_readable_sentence()
    {
        "Must_contains_at_least_one_item_to_order"
            .ToReadableSentence()
            .Should()
            .Be("Must contains at least one item to order");
    }
}