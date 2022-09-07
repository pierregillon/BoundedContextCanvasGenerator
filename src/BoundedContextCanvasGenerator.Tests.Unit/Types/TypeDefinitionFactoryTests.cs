using System.Collections.Generic;
using System.Threading.Tasks;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types;

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
        var data = await Parse(sourceCode);

        data
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {

                A.Class("UserController")
                    .Implementing("System.IEquatable<UserController>")
                    .InAssembly("Test"),

                A.Class("CreateUserCommand")
                    .Implementing("System.IEquatable<CreateUserCommand>")
                    .InAssembly("Test")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController")
                            .Implementing("System.IEquatable<UserController>")
                            .InAssembly("Test"))
                        .FromMethod(A.Method.Named("Post"))
                    )

            });
    }

    [Fact]
    public async Task Do_not_duplicate_instanciator_when_multiple_methods_instanciating()
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

            public async Task Put(string name)
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
        var data = await Parse(sourceCode);

        data
            .Should()
            .BeEquivalentTo(new TypeDefinition[] {

                A.Class("UserController")
                    .Implementing("System.IEquatable<UserController>")
                    .InAssembly("Test"),

                A.Class("CreateUserCommand")
                    .Implementing("System.IEquatable<CreateUserCommand>")
                    .InAssembly("Test")
                    .InstanciatedBy(An.Instanciator
                        .OfType(A.Class("UserController")
                            .Implementing("System.IEquatable<UserController>")
                            .InAssembly("Test"))
                        .FromMethod(A.Method.Named("Post"))
                        .FromMethod(A.Method.Named("Put"))
                    )

            });
    }

    public static async Task<IEnumerable<TypeDefinition>> Parse(params string[] sources)
    {
        var compilation = new SourceCodeCompiler().Compile(sources);

        return await new TypeDefinitionFactory().Build(new[] { compilation });
    }
}