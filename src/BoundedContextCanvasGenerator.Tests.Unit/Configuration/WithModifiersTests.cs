﻿using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Tests.Acceptance.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Configuration;

public class WithModifiersTests
{
    [Fact]
    public void All_modifiers_must_be_present()
    {
        TypeDefinition typeDefinition = TypeDefinitionBuilder.Class("MyClass").Abstract();

        new WithModifiers(TypeModifiers.Abstract)
            .IsMatching(typeDefinition)
            .Should()
            .BeTrue();
    }
}