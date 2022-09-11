using BoundedContextCanvasGenerator.Application.Markdown;
using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class GrynwaldMarkdownGenerator : IMarkdownGenerator
{
    public Task<string> Render(BoundedContextCanvas boundedContextCanvas)
    {
        return Task.Factory.StartNew(() => {
            var document = new MdDocument();
            document.Root.Add(new MdHeading(1, boundedContextCanvas.Name.Value));
            document.Root.AddRange(GenerateSections(boundedContextCanvas));
            return document.ToString();
        });
    }

    private static IEnumerable<MdContainerBlock> GenerateSections(BoundedContextCanvas boundedContextCanvas)
    {
        if (boundedContextCanvas.Definition.IsNotEmpty) {
            yield return GenerateDefinitionSection(boundedContextCanvas.Definition).ToContainerBlock();
        }

        if (boundedContextCanvas.UbiquitousLanguage.IsNotEmpty) {
            yield return GenerateUbiquitousLanguageSection(boundedContextCanvas.UbiquitousLanguage).ToContainerBlock();
        }

        if (boundedContextCanvas.InboundCommunication.IsNotEmpty) {
            yield return GenerateInboundCommunicationSection(boundedContextCanvas.InboundCommunication).ToContainerBlock();
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

    private static IEnumerable<MdBlock> GenerateUbiquitousLanguageSection(UbiquitousLanguage ubiquitousLanguage)
    {
        yield return new MdHeading(2, "Ubiquitous language (Context-specific domain terminology)");
        yield return new MdTable(
            ubiquitousLanguage.Concepts.Select(x => x.Name).Pipe(x => new MdTableRow(x)),
            ubiquitousLanguage.Concepts.Select(x => x.Description).Pipe(x => new MdTableRow(x))
        );
    }

    private static IEnumerable<MdBlock> GenerateInboundCommunicationSection(InboundCommunication inboundCommunication)
    {
        yield return new MdHeading(2, "Inbound communication");
        yield return new InboundCommunicationFlowChartBuilder(inboundCommunication).Build();
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