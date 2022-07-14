using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class ReadmeGenerator
{
    private readonly ITypeDefinitionRepository _repository;
    private readonly IGeneratorConfiguration _configuration;

    public ReadmeGenerator(ITypeDefinitionRepository repository, IGeneratorConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<string> Generate(SolutionName solutionName)
    {
        var types = _repository.GetAll(solutionName);

        var commands = new List<TypeDefinition>();
        var domainEvents = new List<TypeDefinition>();

        await foreach (var typeDefinition in types)
        {
            if (_configuration.CommandsConfiguration.IsEnabled && _configuration.CommandsConfiguration.AllMatching(typeDefinition)) {
                commands.Add(typeDefinition);
            }

            if (_configuration.DomainEventsConfiguration.IsEnabled && _configuration.DomainEventsConfiguration.AllMatching(typeDefinition)) {
                domainEvents.Add(typeDefinition);
            }
        }

        var sections = new[] {
            "# Bounded context canvas",
            _configuration.CommandsConfiguration.IsEnabled ? GenerateCommandsSection(commands).JoinLines() : string.Empty,
            _configuration.DomainEventsConfiguration.IsEnabled ? GenerateDomainEventsSection(domainEvents).JoinLines() : string.Empty,
        };

        return sections.JoinLines();
    }

    private static IEnumerable<string> GenerateCommandsSection(IEnumerable<TypeDefinition> commands)
    {
        yield return "## Commands";

        var anyElement = false;

        foreach (var typeDefinition in commands) {
            anyElement = true;
            yield return $"- {typeDefinition.Name.Value}";
        }

        if (!anyElement) {
            yield return "No commands found";
        }

        yield return Environment.NewLine;
    }

    private static IEnumerable<string> GenerateDomainEventsSection(IEnumerable<TypeDefinition> domainEvents)
    {
        yield return "## Domain events";

        var anyElement = false;

        foreach (var typeDefinition in domainEvents)
        {
            anyElement = true;
            yield return $"- {typeDefinition.Name.Value}";
        }

        if (!anyElement)
        {
            yield return "No domain event found";
        }

        yield return Environment.NewLine;
    }
}