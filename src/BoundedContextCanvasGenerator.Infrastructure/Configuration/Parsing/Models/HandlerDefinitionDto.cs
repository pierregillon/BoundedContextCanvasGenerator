namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public record HandlerDefinitionDto : ISelectable
{
    public string? Selector { get; set; }
    public string? Link { get; set; }
}