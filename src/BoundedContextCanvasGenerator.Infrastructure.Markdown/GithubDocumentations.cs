using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public static class GithubDocumentations
{
    public const string StrategicClassificationDocumentationUrl = "https://github.com/ddd-crew/bounded-context-canvas#strategic-classification";
    public const string DomainRoleDocumentationUrl = "https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md";

    public static readonly IDictionary<Enum, string> StrategicClassificationDefinitions = new Dictionary<Enum, string>() {
        { DomainType.CoreDomain, "a key strategic initiative" },
        { BusinessModel.RevenueGenerator, "people pay directly for this" },
        { Evolution.Commodity, "highly-standardised versions exist" },
    };
}