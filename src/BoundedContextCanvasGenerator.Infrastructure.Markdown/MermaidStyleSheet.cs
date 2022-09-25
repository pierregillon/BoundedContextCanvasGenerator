using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public static class MermaidStyleSheet
{
    private const string CommandColor = "#352ef7";
    private const string DomainEventColor = "#f7962e";
    private const string CollaboratorColor = "#f72ef0";
    private const string PolicyColor = "#E9E705";
    private const string IntegrationEventColor = "#f7962e";

    public static readonly NodeStyle Command = new(GenerateColorStyle(CommandColor));
    public static readonly NodeStyle DomainEvent = new(GenerateColorStyle(DomainEventColor));
    public static readonly NodeStyle FrontCollaborator = new(GenerateColorStyle(CollaboratorColor));
    public static readonly NodeStyle BoundedContextCollaborator = new(GenerateColorStyle(CollaboratorColor));
    public static readonly NodeStyle Policy = new($"{GenerateColorStyle(PolicyColor)}, font-style:italic");
    public static readonly NodeStyle IntegrationEvent = new(GenerateColorStyle(IntegrationEventColor));

    private static string GenerateColorStyle(string color) => $"fill:{MakeTransparent(color)}, stroke:{color}";
    private static string MakeTransparent(string color) => $"{color}22";
}