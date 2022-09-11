using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Configuration.Parsing;

public class TypeDefinitionPredicatesDtoTests
{
    [Fact]
    public void Parse_class()
    {
        var dto = new TypeDefinitionPredicatesDto {
            Selector = "class"
        };

        var predicates = dto.Build();

        predicates
            .Values
            .Should()
            .Equal(new OfType(TypeKind.Class));
    }

    [Fact]
    public void Parse_a_named_class()
    {
        var dto = new TypeDefinitionPredicatesDto {
            Selector = "class named '.*Controller$'"
        };

        var predicates = dto.Build();

        predicates
            .Values
            .Should()
            .Equal(
                new OfType(TypeKind.Class), 
                new NamedLike(".*Controller$")
            );
    }

    [Fact]
    public void Parse_concrete_class()
    {
        var dto = new TypeDefinitionPredicatesDto {
            Selector = "concrete class"
        };

        var predicates = dto.Build();

        predicates
            .Values
            .Should()
            .Equal(
                new WithModifiers(TypeModifiers.Concrete), 
                new OfType(TypeKind.Class)
            );
    }

    [Fact]
    public void Parse_concrete_class_implementing_interface()
    {
        var dto = new TypeDefinitionPredicatesDto {
            Selector = "concrete class implementing 'ICommand<.*>$'"
        };

        var predicates = dto.Build();

        predicates
            .Values
            .Should()
            .Equal(
                new WithModifiers(TypeModifiers.Concrete), 
                new OfType(TypeKind.Class),
                new ImplementsInterfaceMatching("ICommand<.*>$")
            );
    }
}