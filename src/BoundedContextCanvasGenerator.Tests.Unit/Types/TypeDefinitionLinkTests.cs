using System;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types;

public class TypeDefinitionLinkTests
{
    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public void Cannot_instanciate_link_with_empty_value(string empty)
    {
        Action create = () => TypeDefinitionLink.From(empty);

        create.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("wrong")]
    [InlineData("T - test")]
    public void Cannot_instanciate_link_with_invalid_structure(string badStructure)
    {
        Action create = () => TypeDefinitionLink.From(badStructure);

        create.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Not_matching_link_returns_not_linked()
    {
        var link = TypeDefinitionLink.From("T -> .*ICommandHandler<T>$");

        var result = link.AreLinked(A.Class("test"), A.Class("toto"));

        result.Should().BeFalse();
    }

    [Fact]
    public void Types_matching_link_expression_are_linked()
    {
        var link = TypeDefinitionLink.From("T -> .*ICommandHandler<T>$");

        var result = link.AreLinked(
            A.Class("CreateUserCommand"),
            A.Class("CreateUserCommandHandler").Implementing("ICommandHandler<CreateUserCommand>")
        );

        result.Should().BeTrue();
    }

    [Fact]
    public void Arrow_function_can_be_renamed()
    {
        var link = TypeDefinitionLink.From("MyCommand -> .*ICommandHandler<MyCommand>$");

        var result = link.AreLinked(
            A.Class("CreateUserCommand"),
            A.Class("CreateUserCommandHandler").Implementing("ICommandHandler<CreateUserCommand>")
        );

        result.Should().BeTrue();
    }

    [Fact]
    public void Space_can_be_ajusted_arround_arrow()
    {
        var link = TypeDefinitionLink.From("T   ->   .*ICommandHandler<T>$");

        var result = link.AreLinked(
            A.Class("CreateUserCommand"),
            A.Class("CreateUserCommandHandler").Implementing("ICommandHandler<CreateUserCommand>")
        );

        result.Should().BeTrue();
    }

    [Fact]
    public void Dot_are_escaped_in_injected_type()
    {
        var link = TypeDefinitionLink.From("T -> .*ICommandHandler<T>$");

        var result = link.AreLinked(
            A.Class("Namespace.CreateUserCommand"),
            A.Class("Namespace.CreateUserCommandHandler").Implementing("ICommandHandler<Namespace.CreateUserCommand>")
        );

        result.Should().BeTrue();
    }
}