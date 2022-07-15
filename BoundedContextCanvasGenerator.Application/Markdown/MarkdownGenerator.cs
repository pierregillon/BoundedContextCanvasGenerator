using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Markdown;

public class MarkdownGenerator : IMarkdownGenerator
{
    public const string StrategicClassificationDocumentationUrl = "https://github.com/ddd-crew/bounded-context-canvas#strategic-classification";
    public const string DomainRoleDocumentationUrl = "https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md";
    
    private static readonly IDictionary<Enum, string> StrategicClassificationDefinitions = new Dictionary<Enum, string>() {
        { DomainType.CoreDomain, "a key strategic initiative" },
        { BusinessModel.RevenueGenerator, "people pay directly for this" },
        { Evolution.Commodity, "highly-standardised versions exist" },
    };

    public async Task<string> Generate(TypeDefinitionExtraction extraction, ICanvasSettings canvasSettings)
    {
        var sections = new[] {
            $"# {canvasSettings.Name.Value}{Environment.NewLine}",
            canvasSettings.Definition.IsEnabled ? GenerateDefinitionSection(canvasSettings.Definition).JoinLines() : string.Empty,
            extraction.Commands.IsEnabled ? GenerateCommandsSection(extraction.Commands.Values).JoinLines() : string.Empty,
            extraction.DomainEvents.IsEnabled ? GenerateDomainEventsSection(extraction.DomainEvents.Values).JoinLines() : string.Empty,
        };

        return sections.JoinLines();
    }

    private IEnumerable<string> GenerateDefinitionSection(CanvasDefinition canvasDefinition)
    {
        yield return "## Definition";

        if (!canvasDefinition.Description.IsEmpty) {
            yield return $"{Environment.NewLine}### Description";
            yield return $"> {canvasDefinition.Description.Value}";
        }

        yield return GeneratesStrategicClassificationSection(canvasDefinition.StrategicClassification).JoinLines();
        yield return GenerateDomainRoleSection(canvasDefinition.DomainRole).JoinLines();

        yield return Environment.NewLine;
    }

    private static IEnumerable<string> GeneratesStrategicClassificationSection(StrategicClassification strategicClassification)
    {
        if (strategicClassification.IsEmpty) yield break;

        yield return $"{Environment.NewLine}### Strategic classification [(?)]({StrategicClassificationDocumentationUrl})";
        yield return "| Domain | Business Model | Evolution |";
        yield return "| ------------ | ------------ | ------------ |";
        yield return $"| {Generate(strategicClassification.DomainType)} | {Generate(strategicClassification.BusinessModel)} | {Generate(strategicClassification.Evolution)} |";
    }

    private static IEnumerable<string> GenerateDomainRoleSection(DomainRole domainRole)
    {
        if (domainRole.IsEmpty) yield break;

        yield return $"{Environment.NewLine}### Domain role [(?)]({DomainRoleDocumentationUrl}): *{domainRole.Title.Value}*";
        yield return domainRole.Description.Value;
    }

    private static string Generate(Evolution evolution)
        => evolution == Evolution.Unknown ? string.Empty : Format(evolution, FindDefinition(evolution));

    private static string Generate(BusinessModel businessModel)
        => businessModel == BusinessModel.Unknown ? string.Empty : Format(businessModel, FindDefinition(businessModel));

    private static string Generate(DomainType domainType) 
        => domainType == DomainType.Unknown ? string.Empty : Format(domainType, FindDefinition(domainType));

    private static string FindDefinition(Enum @enum) 
        => StrategicClassificationDefinitions.TryGetValue(@enum, out var result) ? result : string.Empty;

    private static string Format(Enum value, string description)
    {
        var finalDescription = string.IsNullOrWhiteSpace(description) 
            ? string.Empty 
            : $"({description})";

        return $"*{value.ToString().ToReadableSentence()}*<br/>{finalDescription}";
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