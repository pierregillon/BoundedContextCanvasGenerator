using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application;

public class MarkdownGenerator
{
    public async Task<string> Generate(TypeDefinitionExtraction extraction)
    {
        var sections = new[] {
            "# Bounded context canvas",
            extraction.Commands.IsEnabled ? GenerateCommandsSection(extraction.Commands.Values).JoinLines() : string.Empty,
            extraction.DomainEvents.IsEnabled ? GenerateDomainEventsSection(extraction.DomainEvents.Values).JoinLines() : string.Empty,
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