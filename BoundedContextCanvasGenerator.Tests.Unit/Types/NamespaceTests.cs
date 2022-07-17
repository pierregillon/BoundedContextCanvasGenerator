using BoundedContextCanvasGenerator.Domain.Types;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types;

public class NamespaceTests
{
    [Fact]
    public void Never_starts_with_empty_namespace()
    {
        var @namespace = new Namespace("Test.Namespace");
        var toCompare = Namespace.Empty;

        var result = @namespace.StartWith(toCompare);

        result
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Empty_namespace_never_starts_with_any_namespace()
    {
        var @namespace = Namespace.Empty;
        var toCompare = new Namespace("Test.Namespace");

        var result = @namespace.StartWith(toCompare);

        result
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Does_not_start_with_another_namespace_when_higher_lengther_than_reference()
    {
        var @namespace = new Namespace("Test.Namespace");
        var toCompare = new Namespace("Test.Namespace.A.X.C");

        var result = @namespace.StartWith(toCompare);

        result
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Does_not_start_with_another_namespace_when_at_least_one_segment_is_different()
    {
        var @namespace = new Namespace("Test.Namespace.A.B.C");
        var toCompare = new Namespace("Test.Namespace.A.X.C");

        var result = @namespace.StartWith(toCompare);

        result
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("Test.Namespace")]
    [InlineData("Test.Namespace.A")]
    [InlineData("Test.Namespace.A.B")]
    [InlineData("Test.Namespace.A.B.C")]
    public void Starts_with_another_namespace_when_all_segments_are_identical(string namespaceToCompare)
    {
        var @namespace = new Namespace("Test.Namespace.A.B.C");
        var toCompare = new Namespace(namespaceToCompare);

        var result = @namespace.StartWith(toCompare);

        result
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Getting_sub_namespaces_enumerates_all_namespaces_from_root()
    {
        var @namespace = new Namespace("Test.Namespace.Commands.Elements");

        @namespace
            .GetSubNamespaces()
            .Should()
            .BeEquivalentTo(new[] {
                new Namespace("Test"),
                new Namespace("Test.Namespace"),
                new Namespace("Test.Namespace.Commands"),
                new Namespace("Test.Namespace.Commands.Elements"),
            });
    }
}