using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types;

public class ClassSourceCodeVisitorTests
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

        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A.Class("CreateUser").InAssembly("Test")
            });
    }

    [Fact]
    public void Indicates_if_class_is_abstract()
    {
        const string sourceCode = @"
                public abstract class CreateUser { }
            ";

        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A.Class("CreateUser").InAssembly("Test").Abstract()
            });
    }

    [Fact]
    public void Extract_class_namespace_from_source_code()
    {
        const string sourceCode = @"
                namespace Test;
                public class CreateUser { }
            ";

        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A.Class("Test.CreateUser").InAssembly("Test")
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

        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A
                    .Class("Test.CreateUser")
                    .InAssembly("Test")
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

        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A
                    .Class("CreateUser")
                    .InAssembly("Test")
                    .Implementing("System.IEquatable<CreateUser>")
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
        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A.Class("Test.CreateUser").InAssembly("Test").Implementing("System.IEquatable<Test.CreateUser>")
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
        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A.Class("Test.CreateUser")
                    .InAssembly("Test")
                    .Implementing("System.IEquatable<Test.CreateUser>")
                    .Implementing("Test.ICommand")
                    .Implementing("Test.IDisposable")
            });
    }

    [Fact]
    public void Extract_documentation_from_class()
    {
        const string sourceCode = @"
        
        /// <summary>
        /// A simple class to create user.
        /// </summary>
        public class CreateUser;
";
        var typeDefinitions = Parse(sourceCode);

        typeDefinitions
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {
                A.Class("CreateUser")
                    .InAssembly("Test")
                    .WithDescription("A simple class to create user.")
            });
    }

    // ----- Private

    public static IEnumerable<TypeDefinition> Parse(params string [] sources)
    {
        var compilation = new SourceCodeCompiler().Compile(sources);

        var results = new List<TypeDefinition>();

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            var visitor = new ClassSourceCodeVisitor(semanticModel, results);

            visitor.Visit(syntaxTree.GetRoot());
        }

        return results;
    }
}