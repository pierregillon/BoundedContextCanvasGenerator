using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LivingDocumentation.Domain;
using NSubstitute;
using Xunit;
using A= BoundedContextCanvasGenerator.Tests.TypeDefinitionBuilder;

namespace BoundedContextCanvasGenerator.Tests
{
    public class ReadmeGeneratorTests
    {
        private static readonly SolutionName SomeSolution = new("some");
        private readonly ITypeDefinitionRepository _repository = Substitute.For<ITypeDefinitionRepository>();
        private readonly ReadmeGenerator _generator;
        private readonly IGeneratorConfiguration _configuration = Substitute.For<IGeneratorConfiguration>();

        public ReadmeGeneratorTests() => _generator = new ReadmeGenerator(_repository, _configuration);

        [Fact]
        public async Task No_commands_renders_not_found()
        {
            var readme = await _generator.Generate(SomeSolution);

            readme.Should().Contain(
@"## Commands
No commands found
");
        }

        [Fact]
        public async Task Commands_matching_pattern_are_listed()
        {
            _configuration.CommandDefinition
                .Returns(new ImplementsInterfaceMatching(".*ICommand"));

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