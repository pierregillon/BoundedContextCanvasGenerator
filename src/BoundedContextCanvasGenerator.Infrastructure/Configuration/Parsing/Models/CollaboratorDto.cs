namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public class CollaboratorDto : ISelectable
{
    public string? Name { get; set; }
    public string? Selector { get; set; }
    public bool IsNotEmpty => Name is not null && Selector is not null;
}