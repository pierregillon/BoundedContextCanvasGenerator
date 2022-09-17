namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public class CollaboratorDefinitionDto : ISelectable
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Selector { get; set; }
    public bool IsNotEmpty => Name is not null && Selector is not null;
}