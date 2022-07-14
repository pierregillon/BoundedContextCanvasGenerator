using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoundedContextCanvasGenerator.Application;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;
using FluentAssertions;
using NSubstitute;
using Xunit;
using A = BoundedContextCanvasGenerator.Tests.Unit.TypeDefinitionBuilder;

namespace BoundedContextCanvasGenerator.Tests.Unit
{
    public class ReadmeGeneratorTests
    {
        private static readonly SolutionName SomeSolution = new("some");
        private readonly ITypeDefinitionRepository _repository = Substitute.For<ITypeDefinitionRepository>();
        private readonly ReadmeGenerator _generator;
        private readonly IGeneratorConfiguration _configuration = Substitute.For<IGeneratorConfiguration>();

        public ReadmeGeneratorTests() => _generator = new ReadmeGenerator(_repository, _configuration);

        [Fact]
        public async Task No_commands_configuration_do_not_generate_commands_section()
        {
            _configuration.CommandsConfiguration.Returns(TypeDefinitionPredicates.Empty());

            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.MyCommand").Implementing("Some.Namespace.ICommand"),
                A.Class("Some.Namespace.MySecondCommand").Implementing("Some.Namespace.ICommand"),
            });

            var readme = await _generator.Generate(SomeSolution);

            readme.Should().NotContain("## Commands");
        }

        [Fact]
        public async Task No_commands_renders_not_found()
        {
            _configuration
                .CommandsConfiguration
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*ICommand")));

            var readme = await _generator.Generate(SomeSolution);

            readme.Should().Contain(
@"## Commands
No commands found
");
        }

        [Fact]
        public async Task Commands_matching_pattern_are_listed()
        {
            _configuration.CommandsConfiguration
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*ICommand")));

            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.MyCommand").Implementing("Some.Namespace.ICommand"),
                A.Class("Some.Namespace.MySecondCommand").Implementing("Some.Namespace.ICommand"),
            });

            var readme = await _generator.Generate(SomeSolution);

            readme.Should().Contain(
@"## Commands
- Some.Namespace.MyCommand
- Some.Namespace.MySecondCommand
");
        }

        private void Define(IEnumerable<TypeDefinition> types)
        {
            _repository
                .GetAll(SomeSolution)
                .Returns(Create(types));
        }

        private static async IAsyncEnumerable<TypeDefinition> Create(IEnumerable<TypeDefinition> elements)
        {
            await Task.Delay(0);

            foreach (var typeDefinition in elements) {
                yield return typeDefinition;
            }
        }
    }
}