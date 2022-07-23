using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using A = BoundedContextCanvasGenerator.Tests.Unit.TypeDefinitionBuilder;
using AMethod = BoundedContextCanvasGenerator.Tests.Unit.Types.MethodInfoBuilder;
using Task = System.Threading.Tasks.Task;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types;

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

    public static VisitedData Visit(params string [] sources)
    {
        sources
            .Should()
            .AllSatisfy(x => x.Should().NotBeNullOrEmpty("without source code there is nothing to test"));

        var visited = VisitAllTrees(sources);

        return visited.Aggregate((x, y) => x + y);
    }

    private static IEnumerable<VisitedData> VisitAllTrees(string[] sources)
    {
        var trees = sources
            .Select(x => x.Trim())
            .Select(x => CSharpSyntaxTree.ParseText(x))
            .ToArray();

        var compilation = CSharpCompilation.Create("Test")
            .WithOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithAllowUnsafe(true)
            )
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(trees);

        foreach (var syntaxTree in trees) {
            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            var visitedData = new VisitedData();
            var visitor = new SourceCodeVisitor(semanticModel, visitedData);

            visitor.Visit(syntaxTree.GetRoot());

            yield return visitedData;
        }
    }
}

public class SourceCodeMethodVisitorTests
{
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
                        new(
                            AMethod.Named("Do"), 
                            new [] {
                                new TypeFullName("Test.CreateUser")
                            }
                        )
                    }
                }
            });
    }    
    
    [Fact]
    public void Extract_attributes_of_the_method()
    {
        const string sourceCode = @"
    namespace Test
    {
        public record CreateUser;
       
        public class CreateUserTests
        {
            [Fact]
            [Trait(""Category"", ""BoundedContextCanvasPolicy"")]
            public void Creates_a_new_user()
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
                    new TypeFullName("Test.CreateUserTests"),
                    new List<MethodDefinition> {
                        new(
                            AMethod
                                .Named("Creates_a_new_user")
                                .WithAttribute("Fact")
                                .WithAttribute("Trait(\"Category\", \"BoundedContextCanvasPolicy\")"), 
                            new [] {
                                new TypeFullName("Test.CreateUser")
                            }
                        )
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
                        new(AMethod.Named("Do"), new [] {
                            new TypeFullName("CreateUser"),
                            new TypeFullName("DeleteUser")
                        }),
                        new(AMethod.Named("Do2"), new [] {
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
                        new(AMethod.Named("Do"), new [] {
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
                        new(AMethod.Named("Do"), new [] {
                            new TypeFullName("CreateUser"),
                        }),
                        new(AMethod.Named("Create"), new [] {
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
                        new(AMethod.Named("Do"), new [] {
                            new TypeFullName("CreateUser"),
                        }),
                        new(AMethod.Named("Create"), new [] {
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

    [Fact]
    public void Navigate_to_another_tree()
    {
        const string sourceCode = @"
        public class CreateUserService
        {
            public void Do()
            {
                var command = Builder.Build();
            }
        }
";
        const string sourceCode2 = @"

        public static class Builder 
        {
            public static CreateUser Build()
            {
                return new CreateUser();
            }
        }

        public record CreateUser;
";
        var data = Visit(sourceCode, sourceCode2);

        data
            .Methods
            .Should()
            .BeEquivalentTo(new Dictionary<TypeFullName, List<MethodDefinition>> {
                {
                    new TypeFullName("CreateUserService"),
                    new List<MethodDefinition> {
                        new(AMethod.Named("Do"), new[] {
                            new TypeFullName("CreateUser"),
                        }),
                    }
                }, {
                    new TypeFullName("Builder"),
                    new List<MethodDefinition> {
                        new(AMethod.Named("Build"), new[] {
                            new TypeFullName("CreateUser"),
                        }),
                    }
                }
            });
    }

    public static VisitedData2 Visit(params string[] sources)
    {
        sources
            .Should()
            .AllSatisfy(x => x.Should().NotBeNullOrEmpty("without source code there is nothing to test"));

        var visited = VisitAllTrees(sources);

        return visited;
    }

    private static VisitedData2 VisitAllTrees(string[] sources)
    {
        var trees = sources
            .Select(x => x.Trim())
            .Select(x => CSharpSyntaxTree.ParseText(x))
            .ToArray();

        var compilation = CSharpCompilation.Create("Test")
            .WithOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithAllowUnsafe(true)
            )
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(trees);

        var semanticModels = trees.Select(x => compilation.GetSemanticModel(x, true)).ToArray();

        var visitedData = new VisitedData2();
        var visitor = new SourceCodeMethodVisitor(semanticModels, visitedData);

        foreach (var syntaxTree in trees)
        {
            visitor.Visit(syntaxTree.GetRoot());
        }

        return visitedData;
    }
}

public class TypeDefinitionFactoryTests
{
    [Fact]
    public async Task Build_type_definition_including_instanciators()
    {
        const string sourceCode = @"
        using System.Threading.Tasks;
        
        public record UserController(ICommandDispatcher Dispatcher)
        {
            public async Task Post(string name)
            {
                var command = new CreateUserCommand(name);
                await Dispatcher.Dispatch(command);
            }
        }

        public record CreateUserCommand(string name);

        public interface ICommandDispatcher
        {
            public Task Dispatch(object command);
        }
";
        var data = await Visit(sourceCode);

        data
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {

                A.Class("UserController")
                    .Implementing("System.IEquatable<UserController>")
                    .InAssembly("Test"),

                A.Class("CreateUserCommand")
                    .Implementing("System.IEquatable<CreateUserCommand>")
                    .InAssembly("Test")
                    .InstanciatedBy("Post", 
                        A.Class("UserController")
                            .Implementing("System.IEquatable<UserController>")
                            .InAssembly("Test")
                    )

            });
    }

    public static async Task<IEnumerable<TypeDefinition>> Visit(params string[] sources)
    {
        sources
            .Should()
            .AllSatisfy(x => x.Should().NotBeNullOrEmpty("without source code there is nothing to test"));

        var trees = sources
            .Select(x => x.Trim())
            .Select(x => CSharpSyntaxTree.ParseText(x))
            .ToArray();

        var compilation = CSharpCompilation.Create("Test")
            .WithOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithAllowUnsafe(true)
            )
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(trees);

        return await new TypeDefinitionFactory().Build(new[] { compilation });
    }
}

public class MethodInfoBuilder
{
    private MethodName _name;
    private List<MethodAttribute> _attributes = new();

    public static MethodInfoBuilder Named(string name) => new MethodInfoBuilder().WithName(name);

    public MethodInfoBuilder WithName(string name)
    {
        this._name = new MethodName(name);
        return this;
    }

    public MethodInfoBuilder WithAttribute(string attribute)
    {
        this._attributes.Add(new MethodAttribute(attribute));
        return this;
    }

    public MethodInfo Build() => new(this._name, this._attributes);

    public static implicit operator MethodInfo(MethodInfoBuilder builder) => builder.Build();
}