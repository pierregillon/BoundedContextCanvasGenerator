using System.Linq;
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

    public class Converting_to_PascalCase
    {
        [Fact]
        public void Removes_space()
        {
            const string value = "It is time to get things done";

            value.ToPascalCase().Should().NotContain(" ");
        }

        [Theory]
        [InlineData("hellow")]
        [InlineData("aBB")]
        public void Always_start_with_upper_case_letter(string value)
        {
            value.ToPascalCase().First().ToString().Should().BeUpperCased();
        }

        [Fact]
        public void Makes_character_after_space_character_upper_cased()
        {
            const string value = "It is time to get things done";

            value.ToPascalCase().Should().Be("ItIsTimeToGetThingsDone");
        }
    }
}