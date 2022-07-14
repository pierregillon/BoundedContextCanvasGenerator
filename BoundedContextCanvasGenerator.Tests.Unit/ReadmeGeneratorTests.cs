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

        public ReadmeGeneratorTests()
        {
            _generator = new ReadmeGenerator(_repository, _configuration);

            _configuration
                .CommandsConfiguration
                .Returns(TypeDefinitionPredicates.Empty());

            _configuration
                .DomainEventsConfiguration
                .Returns(TypeDefinitionPredicates.Empty());
        }

        [Fact]
        public async Task No_commands_configuration_do_not_generate_commands_section()
        {
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
            _configuration
                .CommandsConfiguration
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

        [Fact]
        public async Task No_domain_events_configuration_do_not_generate_domain_events_section()
        {
            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.MyDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
                A.Class("Some.Namespace.MySecondDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
            });

            var readme = await _generator.Generate(SomeSolution);

            readme.Should().NotContain("## Domain events");
        }

        [Fact]
        public async Task No_domain_events_matching_render_empty_section()
        {
            _configuration
                .DomainEventsConfiguration
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*IDomainEvent")));

            var readme = await _generator.Generate(SomeSolution);

            readme.Should().Contain(
                @"## Domain events
No domain event found
");
        }

        [Fact]
        public async Task Domain_events_matching_pattern_are_listed()
        {
            _configuration
                .DomainEventsConfiguration
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*IDomainEvent")));

            Define(new TypeDefinition[] {
                A.Class("Some.Namespace.MyDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
                A.Class("Some.Namespace.MySecondDomainEvent").Implementing("Some.Namespace.IDomainEvent"),
            });

            var readme = await _generator.Generate(SomeSolution);

            readme.Should().Contain(
@"## Domain events
- Some.Namespace.MyDomainEvent
- Some.Namespace.MySecondDomainEvent
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