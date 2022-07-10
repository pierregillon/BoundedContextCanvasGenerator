using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using LivingDocumentation.Domain;

namespace BoundedContextCanvasGenerator;

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
            if (_configuration.CommandDefinition.IsMatching(typeDefinition)) {
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

public interface IGeneratorConfiguration
{
    public IGeneratorDefinition CommandDefinition { get; }
}

public interface IGeneratorDefinition
{
    bool IsMatching(TypeDefinition type);
}

public class ImplementsInterfaceMatching : IGeneratorDefinition
{
    private readonly Regex _regex;

    public ImplementsInterfaceMatching(string pattern) => _regex = new Regex(pattern, RegexOptions.Compiled);

    public bool IsMatching(TypeDefinition type) => type.ImplementedInterfaces.Any(x => _regex.IsMatch(x.Value));
}

public static class StringExtensions
{
    public static string JoinLines(this IEnumerable<string> elements) 
        => string.Join(Environment.NewLine, elements);
}