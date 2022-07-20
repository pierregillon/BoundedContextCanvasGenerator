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

            var data = Visit(sourceCode);

            data
                .TypeDefinitions
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

            var data = Visit(sourceCode);

            data
                .TypeDefinitions
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

            var data = Visit(sourceCode);

            data
                .TypeDefinitions
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

            var data = Visit(sourceCode);

            data
                .TypeDefinitions
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

            var data = Visit(sourceCode);

            data
                .TypeDefinitions
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
            var data = Visit(sourceCode);

            data
                .TypeDefinitions
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
            var data = Visit(sourceCode);

            data
                .TypeDefinitions
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
            var data = Visit(sourceCode);

            data
                .TypeDefinitions
                .Should()
                .BeEquivalentTo(new TypeDefinition[] {
                    A.Class("CreateUser")
                        .InAssembly("Test")
                        .WithDescription("A simple class to create user.")
                });
        }

        [Fact]
        public void Extract_a_method_instanciating_an_known_type()
        {
            const string sourceCode = @"
    namespace Test
    {
        public record CreateUser;

        public class CreateUserService
        {
            public void Do()
            {
                var command = new CreateUser();
            }
        }
    }
";
            var data = Visit(sourceCode);

            data
                .Methods
                .Should()
                .BeEquivalentTo(new Dictionary<TypeFullName, List<MethodDefinition>> {
                    {
                        new TypeFullName("Test.CreateUserService"), 
                        new List<MethodDefinition> {
                            new MethodDefinition("Do", new [] {
                                new TypeFullName("Test.CreateUser")
                            })
                        }
                    }
                });
        }

        [Fact]
        public void Extract_a_method_instanciating_multiple_types()
        {
            const string sourceCode = @"
    public record CreateUser;
    public record DeleteUser;

    public class CreateUserService
    {
        public void Do()
        {
            var command1 = new CreateUser();
            var command2 = new DeleteUser();
        }

        public void Do2()
        {
            var command1 = new CreateUser(), command2 = new DeleteUser();
        }
    }
";
            var data = Visit(sourceCode);
            data
                .Methods
                .Should()
                .BeEquivalentTo(new Dictionary<TypeFullName, List<MethodDefinition>> {
                    {
                        new TypeFullName("CreateUserService"), 
                        new List<MethodDefinition> {
                            new("Do", new [] {
                                new TypeFullName("CreateUser"),
                                new TypeFullName("DeleteUser")
                            }),
                            new("Do2", new [] {
                                new TypeFullName("CreateUser"),
                                new TypeFullName("DeleteUser")
                            })
                        }
                    }
                });
        }
        
        [Fact]
        public void Extract_method_by_traversing_statements()
        {
            const string sourceCode = @"
    public record CreateUser;

    public class CreateUserService
    {
        public void Do()
        {
            if(true) {
                if(true) {
                    var command1 = new CreateUser();
                }
            }
        }
    }
";
            var data = Visit(sourceCode);
            data
                .Methods
                .Should()
                .BeEquivalentTo(new Dictionary<TypeFullName, List<MethodDefinition>> {
                    {
                        new TypeFullName("CreateUserService"), 
                        new List<MethodDefinition> {
                            new("Do", new [] {
                                new TypeFullName("CreateUser"),
                            })
                        }
                    }
                });
        }
        
        [Fact]
        public void Extract_method_by_traversing_method_call()
        {
            const string sourceCode = @"
    public record CreateUser;

    public class CreateUserService
    {
        public void Do()
        {
            Create();
        }

        private void Create()
        {
            var command = new CreateUser();
        }
    }
";
            var data = Visit(sourceCode);
            data
                .Methods
                .Should()
                .BeEquivalentTo(new Dictionary<TypeFullName, List<MethodDefinition>> {
                    {
                        new TypeFullName("CreateUserService"), 
                        new List<MethodDefinition> {
                            new("Do", new [] {
                                new TypeFullName("CreateUser"),
                            }),
                            new("Create", new [] {
                                new TypeFullName("CreateUser"),
                            })
                        }
                    }
                });
        }
        
        [Fact]
        public void Extract_method_by_traversing_get_method()
        {
            const string sourceCode = @"
    public record CreateUser;

    public class CreateUserService
    {
        public void Do()
        {
            var command = Create();
        }

        private CreateUser Create()
        {
            return new CreateUser();
        }
    }
";
            var data = Visit(sourceCode);
            data
                .Methods
                .Should()
                .BeEquivalentTo(new Dictionary<TypeFullName, List<MethodDefinition>> {
                    {
                        new TypeFullName("CreateUserService"), 
                        new List<MethodDefinition> {
                            new("Do", new [] {
                                new TypeFullName("CreateUser"),
                            }),
                            new("Create", new [] {
                                new TypeFullName("CreateUser"),
                            })
                        }
                    }
                });
        }

        [Fact]
        public void Ignores_method_instanciating_not_declared_type()
        {
            const string sourceCode = @"
    namespace Test
    {
        public record CreateUser;

        public class CreateUserService
        {
            public void Do()
            {
                var command = new object();
            }
        }
    }
";
            var data = Visit(sourceCode);

            data
                .Methods
                .Should()
                .BeEmpty();
        }

        public static VisitedData Visit(string source)
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

            var visitedData = new VisitedData();
            var visitor = new SourceCodeVisitor(semanticModel, visitedData);

            visitor.Visit(syntaxTree.GetRoot());

            return visitedData;
        }
    }
}