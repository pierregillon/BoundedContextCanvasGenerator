using System;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Configuration;

public class NamedLikeTests
{

    [Fact]
    public void Two_named_like_are_equals()
    {
        Func<NamedLike> build = () => new NamedLike(".*Controller$");

        build().Should().Be(build());
    }
}