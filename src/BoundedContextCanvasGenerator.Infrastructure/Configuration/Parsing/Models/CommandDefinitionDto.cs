namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public record CommandDefinitionDto : ISelectable
{
    public string? Selector { get; set; }
    public HandlerDefinitionDto? Handler { get; set; }
}