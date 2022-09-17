using System.Linq;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types;

public class MethodSourceCodeVisitorTests
{
    [Fact]
    public void Ignores_methods_not_instanciating_types()
    {
        const string sourceCode = @"
    namespace Test
    {
        public class CreateUserService
        {
            public void Do()
            {
            }
        }
    }
";
        var methodDefinitions = Parse(sourceCode);

        methodDefinitions
            .Should()
            .Be(A.MethodDefinitions.Empty());
    }

    [Fact]
    public void Extract_a_method_instanciating_a_declared_type()
    {
        const string sourceCode = @"

        public record CreateUserCommand;

        public class CreateUserService
        {
            public void Do()
            {
                var command = new CreateUserCommand();
            }
        }
";
        var methodDefinitions = Parse(sourceCode);

        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserService")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Do"))
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Theory]
    [InlineData("object")]
    [InlineData("string")]
    [InlineData("int")]
    public void Ignores_method_instanciating_a_not_declared_type(string systemType)
    {
        string sourceCode = @$"
        public record CreateUser;

        public class CreateUserService
        {{
            public void Do()
            {{
                var command = new {systemType}();
            }}
        }}
";
        var methodDefinitions = Parse(sourceCode);

        methodDefinitions
            .Should()
            .Be(A.MethodDefinitions.Empty());
    }

    [Fact]
    public void Extract_attributes_of_the_method()
    {
        const string sourceCode = @"
        public record CreateUserCommand;
       
        public class CreateUserTests
        {
            [Fact]
            [Trait(""Category"", ""BoundedContextCanvasPolicy"")]
            public void Creates_a_new_user()
            {
                var command = new CreateUserCommand();
            }
        }
";
        var methodDefinitions = Parse(sourceCode);

        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserTests")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method
                                .Named("Creates_a_new_user")
                                .WithAttribute("Fact")
                                .WithAttribute("Trait(\"Category\", \"BoundedContextCanvasPolicy\")")
                            )
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Fact]
    public void Extract_methods_instanciating_multiple_declared_types()
    {
        const string sourceCode = @"
    public record CreateUserCommand;
    public record DeleteUserCommand;

    public class CreateUserService
    {
        public void Do()
        {
            var command1 = new CreateUserCommand();
            var command2 = new DeleteUserCommand();
        }

        public void Do2()
        {
            var command1 = new CreateUserCommand(), command2 = new DeleteUserCommand();
        }
    }
";
        var methodDefinitions = Parse(sourceCode);
        
        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserService")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Do"))
                            .Instanciating("CreateUserCommand")
                            .Instanciating("DeleteUserCommand")
                        )
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Do2"))
                            .Instanciating("CreateUserCommand")
                            .Instanciating("DeleteUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Fact]
    public void Extract_declared_types_instanciated_by_method_by_traversing_statements()
    {
        const string sourceCode = @"
    public record CreateUserCommand;

    public class CreateUserService
    {
        public void Do()
        {
            if(true) {
                if(true) {
                    var command1 = new CreateUserCommand();
                }
            }
        }
    }
";
        var methodDefinitions = Parse(sourceCode);
        
        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserService")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Do"))
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Fact]
    public void Extract_declared_types_instanciated_by_method_by_traversing_method_invocation()
    {
        const string sourceCode = @"
    public record CreateUserCommand;

    public class CreateUserService
    {
        public void Do()
        {
            Create();
        }

        private void Create()
        {
            var command = new CreateUserCommand();
        }
    }
";
        var methodDefinitions = Parse(sourceCode);
        
        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserService")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Do"))
                            .Instanciating("CreateUserCommand")
                        )
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Create"))
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Fact]
    public void Extract_declared_types_instanciated_by_method_by_traversing_method_invocation2()
    {
        const string sourceCode = @"
    public record CreateUserCommand;
    
    [ApiController]
    [Route(""[controller]"")]
    public class CatalogController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public CatalogController(ICommandDispatcher commandDispatcher) => _commandDispatcher = commandDispatcher;

        [HttpPost]
        public async Task CreateUser([FromBody] CreateUserModel model)
        {
            await this._commandDispatcher.Dispatch(new CreateUserCommand(new CatalogName(model.Name), new CatalogDescription(model.Description)));
        }
    }
";
        var methodDefinitions = Parse(sourceCode);
        
        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CatalogController")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("CreateUser").WithAttribute("HttpPost"))
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Fact]
    public void Extract_declared_types_instanciated_by_method_by_traversing_return_method_invocation()
    {
        const string sourceCode = @"
    public record CreateUserCommand;

    public class CreateUserService
    {
        public void Do()
        {
            var command = Create();
        }

        private CreateUser Create()
        {
            return new CreateUserCommand();
        }
    }
";
        var methodDefinitions = Parse(sourceCode);
        
        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserService")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Do"))
                            .Instanciating("CreateUserCommand")
                        )
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Create"))
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Fact]
    public void Extract_declared_types_instanciated_by_method_by_traversing_different_source_codes()
    {
        const string commandDefinitionSourceCode = 
@"
        public record CreateUserCommand;
";

        const string serviceDefinitionSourceCode = 
@"
        public class CreateUserService
        {
            public void Do()
            {
                var command = Builder.Build();
            }
        }
";
        const string builderDefinitionSourceCode = 
@"
        public static class Builder 
        {
            public static CreateUser Build()
            {
                return new CreateUserCommand();
            }
        }

";
        var methodDefinitions = Parse(
            commandDefinitionSourceCode, 
            serviceDefinitionSourceCode, 
            builderDefinitionSourceCode
        );

        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserService")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Do"))
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .With(A.Type
                        .Named("Builder")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Build"))
                            .Instanciating("CreateUserCommand")
                        )
                    )
                    .Build()
            );
    }

    [Fact]
    public void Extract_instanciating_types_from()
    {
        const string sourceCode = @"
    public record CreateUserCommand;

    public record CreateUserCommandHandler
    {
        public void Handle(CreateUserCommand command)
        {
            var user = new User();
        }
    }

    public class User
    {
        public User()
        {
            this.StoreEvent(new UserCreated());
        }

        private void StoreEvent(object @event) { }
    }

    public record UserCreated;
";
        var methodDefinitions = Parse(sourceCode);

        methodDefinitions
            .Should()
            .Be(
                A.MethodDefinitions
                    .With(A.Type
                        .Named("CreateUserCommandHandler")
                        .WithMethod(A.MethodDefinition
                            .WithInfo(A.Method.Named("Handle"))
                            .Instanciating("User")
                            .Instanciating("UserCreated")
                        )
                    )
                    .Build()
            );
    }

    // ----- Private

    public static MethodDefinitions Parse(params string[] sources)
    {
        var compilation = new SourceCodeCompiler().Compile(sources);

        var semanticModels = compilation.SyntaxTrees.Select(x => compilation.GetSemanticModel(x, true)).ToArray();

        var methodDefinitions = new MethodDefinitions();

        var visitor = new MethodSourceCodeVisitor(semanticModels, methodDefinitions);

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            visitor.Visit(syntaxTree.GetRoot());
        }

        return methodDefinitions;
    }
}