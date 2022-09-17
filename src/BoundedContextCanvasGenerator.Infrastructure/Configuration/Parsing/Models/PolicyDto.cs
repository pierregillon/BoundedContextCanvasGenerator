namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public class PolicyDto
{
    public string? MethodAttributePattern { get; set; }
    public bool IsNotEmpty => MethodAttributePattern is not null;
}