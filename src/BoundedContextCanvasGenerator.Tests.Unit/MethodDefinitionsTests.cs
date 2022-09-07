using System;
using System.Linq;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit
{
    public class MethodDefinitionsTests
    {
        [Fact]
        public void test()
        {
            var def = new MethodDefinitions();
            def.AddMethod(new TypeFullName("CatalogController"), new MethodDefinition(new MethodInfo(new MethodName("Create"), Array.Empty<MethodAttribute>()), new[] { new TypeFullName("CreateCatalogCommand") }));
            def.AddMethod(new TypeFullName("CatalogItemController"), new MethodDefinition(new MethodInfo(new MethodName("Create"), Array.Empty<MethodAttribute>()), new[] { new TypeFullName("CreateCatalogItemCommand") }));

            var results = def.FindInstanciators(A.Class("CreateCatalogCommand"));

            results.Single().Item2.Should().HaveCount(1);
        }
    }
}