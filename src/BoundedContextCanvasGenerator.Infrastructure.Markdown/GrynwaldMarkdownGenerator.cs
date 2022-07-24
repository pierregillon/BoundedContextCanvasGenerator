using BoundedContextCanvasGenerator.Application.Extractions;
using BoundedContextCanvasGenerator.Application.Markdown;
using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class GrynwaldMarkdownGenerator : IMarkdownGenerator
{
    public Task<string> Generate(TypeDefinitionExtraction extraction, ICanvasSettings canvasSettings)
    {
        var document = new MdDocument();
        
        document.Root.Add(new MdHeading(1, canvasSettings.Name.Value));
        document.Root.AddRange(GenerateSections(extraction, canvasSettings));
        
        return document
            .ToString()
            .Pipe(Task.FromResult);
    }

    private static IEnumerable<MdContainerBlock> GenerateSections(TypeDefinitionExtraction extraction, ICanvasSettings canvasSettings)
    {
        if (canvasSettings.Definition.IsEnabled) {
            yield return GenerateDefinitionSection(canvasSettings.Definition).ToContainerBlock();
        }

        if (extraction.Aggregates.IsEnabled) {
            yield return GenerateUbiquitousLanguageSection(extraction.Aggregates.Values).ToContainerBlock();
        }

        if (extraction.Commands.IsEnabled) {
            yield return GenerateInboundCommunicationSection(extraction.Commands.Values, canvasSettings).ToContainerBlock();
        }

        if (extraction.DomainEvents.IsEnabled) {
            yield return GenerateDomainEventsSection(extraction.DomainEvents.Values).ToContainerBlock();
        }
    }

    private static IEnumerable<MdBlock> GenerateDefinitionSection(CanvasDefinition canvasDefinition)
    {
        yield return new MdHeading(2, "Definition");

        if (!canvasDefinition.Description.IsEmpty) {
            yield return new MdHeading(3, "Description");
            yield return new MdBlockQuote(canvasDefinition.Description.Value);
        }

        yield return GeneratesStrategicClassificationSection(canvasDefinition.StrategicClassification).ToContainerBlock();
        yield return GenerateDomainRoleSection(canvasDefinition.DomainRole).ToContainerBlock();
    }

    private static IEnumerable<MdBlock> GeneratesStrategicClassificationSection(StrategicClassification strategicClassification)
    {
        if (strategicClassification.IsEmpty) yield break;

        yield return new MdHeading(3, "Strategic classification ", new MdLinkSpan("(?)", GithubDocumentations.StrategicClassificationDocumentationUrl));
        yield return new MdTable(
            new MdTableRow("Domain", "Business Model", "Evolution"),
            new MdTableRow(
                GetNameAndDefinition(strategicClassification.DomainType),
                GetNameAndDefinition(strategicClassification.BusinessModel),
                GetNameAndDefinition(strategicClassification.Evolution)
            )
        );
    }

    private static IEnumerable<MdBlock> GenerateDomainRoleSection(DomainRole domainRole)
    {
        if (domainRole.IsEmpty) yield break;

        yield return new MdHeading(3, "Domain role ", new MdLinkSpan("(?)", GithubDocumentations.DomainRoleDocumentationUrl), ": ", new MdEmphasisSpan(domainRole.Title.Value));
        yield return new MdParagraph(domainRole.Description.Value);
    }

    private static IEnumerable<MdBlock> GenerateUbiquitousLanguageSection(IReadOnlyCollection<TypeDefinition> aggregates)
    {
        yield return new MdHeading(2, "Ubiquitous language (Context-specific domain terminology)");

        if (aggregates.Any()) {
            yield return new MdTable(
                aggregates.Select(x => x.FullName.Name.ToReadableSentence()).Pipe(x => new MdTableRow(x)),
                aggregates.Select(x => x.Description.Value).Pipe(x => new MdTableRow(x))
            );
        }
        else {
            yield return new MdParagraph("No ubiquitous language found");
        }
    }

    private static IEnumerable<MdBlock> GenerateInboundCommunicationSection(IReadOnlyCollection<TypeDefinition> commands, ICanvasSettings canvasSettings)
    {
        yield return new MdHeading(2, "Inbound communication");

        if (!commands.Any()) {
            yield return new MdParagraph("No inbound communication found");
        }
        else {
            //yield return InboundCommunicationFlowChartBuilder.From(commands).Build(true);
            yield return new InboundCommunicationFlowChartBuilder2(
                canvasSettings.InboundCommunication.CollaboratorDefinitions.Select(x => new CollaboratorDefinition2(x.Name, x.Predicates)).ToArray(),
                canvasSettings.InboundCommunication.PolicyDefinitions
            ).Build(commands);
        }
    }

    private static IEnumerable<MdBlock> GenerateDomainEventsSection(IReadOnlyCollection<TypeDefinition> domainEvents)
    {
        yield return new MdHeading(2, "Domain events");

        if (!domainEvents.Any()) {
            yield return new MdParagraph("No domain event found");
        }
        else {
            yield return new MdBulletList(domainEvents.Select(x => new MdListItem(x.FullName.Value)));
        }
    }

    private static MdCompositeSpan GetNameAndDefinition(Enum @enum) =>
        new(
            new MdEmphasisSpan(@enum.ToString().ToReadableSentence()),
            SpecialSpan.NewLine,
            new MdTextSpan(FindDocumentation(@enum))
        );
    
    private static string FindDocumentation(Enum @enum)
    {
        var documentation = GithubDocumentations.StrategicClassificationDefinitions.TryGetValue(@enum, out var result) ? result : string.Empty;

        return string.IsNullOrWhiteSpace(documentation)
            ? string.Empty
            : $"({documentation})";
    }
}