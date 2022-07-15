using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using A = BoundedContextCanvasGenerator.Tests.Unit.TypeDefinitionBuilder;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types
{
    public class SourceCodeVisitorTests
    {
        private static IEnumerable<object[]> AccessibilityLevels()
        {
            yield return new[] { "public" };
            yield return new[] { "internal" };
            yield return new[] { "private" };
            yield return new[] { "protected" };
        }

        [Theory]
        [MemberData(nameof(AccessibilityLevels))]
        public void Extract_class_name_from_source_code(string accessibility)
        {
            var sourceCode = @$"
                {accessibility} class CreateUser {{ }}
            ";

            var typeDefinitions = Visit(sourceCode);

            typeDefinitions.Should().BeEquivalentTo(new TypeDefinition[] {
                A.Class("CreateUser")
            });
        }

        [Fact]
        public void Extract_class_namespace_from_source_code()
        {
            const string sourceCode = @"
                namespace Test;
                public class CreateUser { }
            ";

            var typeDefinitions = Visit(sourceCode);

            typeDefinitions.Should().BeEquivalentTo(new TypeDefinition[] {
                A.Class("Test.CreateUser")
            });
        }

        [Fact]
        public void Extract_class_implemented_interfaces_from_source_code()
        {
            const string sourceCode = @"
                namespace Test;
                public interface ICommand { }
                public interface IDisposable { }
                public class CreateUser : ICommand, IDisposable { }
            ";

            var typeDefinitions = Visit(sourceCode);

            typeDefinitions.Should().BeEquivalentTo(new TypeDefinition[] {
                A
                    .Class("Test.CreateUser")
                    .Implementing("Test.ICommand")
                    .Implementing("Test.IDisposable")
            });
        }

        [Theory]
        [MemberData(nameof(AccessibilityLevels))]
        public void Extract_record_name_from_source_code(string accessibility)
        {
            var sourceCode = @$"
                {accessibility} record CreateUser;
            ";

            var typeDefinitions = Visit(sourceCode);

            typeDefinitions.Should().BeEquivalentTo(new TypeDefinition[] {
                A.Class("CreateUser").Implementing("System.IEquatable<CreateUser>")
            });
        }

        [Fact]
        public void Extract_record_namespace_from_source_code()
        {
            const string sourceCode = @"
    namespace Test
    {
        public record CreateUser;
    }
";
            var typeDefinitions = Visit(sourceCode);

            typeDefinitions.Should().BeEquivalentTo(new TypeDefinition[] {
                A.Class("Test.CreateUser").Implementing("System.IEquatable<Test.CreateUser>")
            });
        }

        [Fact]
        public void Extract_record_implemented_interfaces_from_source_code()
        {
            const string sourceCode = @"
    namespace Test
    {
        public interface ICommand { }
        public interface IDisposable { }
        public record CreateUser : ICommand, IDisposable;
    }
";
            var typeDefinitions = Visit(sourceCode);

            typeDefinitions.Should().BeEquivalentTo(new TypeDefinition[] {
                A.Class("Test.CreateUser")
                    .Implementing("System.IEquatable<Test.CreateUser>")
                    .Implementing("Test.ICommand")
                    .Implementing("Test.IDisposable")
            });
        }

        public static IEnumerable<TypeDefinition> Visit(string source)
        {
            source
                .Should()
                .NotBeNullOrWhiteSpace("without source code there is nothing to test");

            var syntaxTree = CSharpSyntaxTree.ParseText(source.Trim());

            var compilation = CSharpCompilation.Create("Test")
                .WithOptions(
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                        .WithAllowUnsafe(true)
                )
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(syntaxTree);

            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            var types = new List<TypeDefinition>();
            var visitor = new SourceCodeVisitor(semanticModel, types);

            visitor.Visit(syntaxTree.GetRoot());

            return types;
        }
    }
}