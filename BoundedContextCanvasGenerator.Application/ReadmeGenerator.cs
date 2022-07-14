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

        await foreach (var typeDefinition in types)
        {
            if (_configuration.CommandDefinitions.All(x=> x.IsMatching(typeDefinition))) {
                commands.Add(typeDefinition);
            }
        }

        var sections = new[] {
            "# Bounded context canvas",
            GenerateCommandsSection(commands).JoinLines()
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
}