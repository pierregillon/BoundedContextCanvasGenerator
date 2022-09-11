using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Tests.Acceptance.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Configuration;

public class OfTypeTests
{
    [Fact]
    public void A_class_does_not_match_of_type_interface_predicate()
    {
        TypeDefinition typeDefinition = TypeDefinitionBuilder.Class("MyClass");

        new OfType(TypeKind.Interface)
            .IsMatching(typeDefinition)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void A_class_matches_of_type_class_predicate()
    {
        TypeDefinition typeDefinition = TypeDefinitionBuilder.Class("MyClass");

        new OfType(TypeKind.Class)
            .IsMatching(typeDefinition)
            .Should()
            .BeTrue();
    }
}