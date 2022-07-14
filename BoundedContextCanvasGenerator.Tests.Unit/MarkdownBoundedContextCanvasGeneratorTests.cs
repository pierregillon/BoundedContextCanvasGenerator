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
    public class MarkdownBoundedContextCanvasGeneratorTests
    {
        private static readonly SolutionPath SomeSolution = new("some");
        private static readonly ConfigurationPath SomeConfigurationPath = new("some");

        private readonly MarkdownBoundedContextCanvasGenerator _generator;
        private readonly ITypeDefinitionRepository _typeDefinitionRepository = Substitute.For<ITypeDefinitionRepository>();
        private readonly IConfigurationRepository _configurationRepository = Substitute.For<IConfigurationRepository>();
        private readonly IGeneratorConfiguration _configuration = Substitute.For<IGeneratorConfiguration>();

        public MarkdownBoundedContextCanvasGeneratorTests()
        {
            _generator = new MarkdownBoundedContextCanvasGenerator(_typeDefinitionRepository, _configurationRepository);

            _configurationRepository
                .Get(Arg.Any<ConfigurationPath>())
                .Returns(_configuration);

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

            var markdown = await GenerateMarkdown();

            markdown.Should().NotContain("## Commands");
        }

        [Fact]
        public async Task No_commands_renders_not_found()
        {
            _configuration
                .CommandsConfiguration
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*ICommand")));

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
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

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
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

            var markdown = await GenerateMarkdown();

            markdown.Should().NotContain("## Domain events");
        }

        [Fact]
        public async Task No_domain_events_matching_render_empty_section()
        {
            _configuration
                .DomainEventsConfiguration
                .Returns(TypeDefinitionPredicates.From(new ImplementsInterfaceMatching(".*IDomainEvent")));

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
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

            var markdown = await GenerateMarkdown();

            markdown.Should().Contain(
@"## Domain events
- Some.Namespace.MyDomainEvent
- Some.Namespace.MySecondDomainEvent
");
        }

        private Task<string> GenerateMarkdown() => _generator.Generate(SomeSolution, SomeConfigurationPath);

        private void Define(IEnumerable<TypeDefinition> types)
        {
            _typeDefinitionRepository
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