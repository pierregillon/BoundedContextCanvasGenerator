namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public record IntegrationEventDefinitionDto : ISelectable
{
    public string? Selector { get; set; }
}