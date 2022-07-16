namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class ConfigurationDto
{
    public string? Name { get; set; }
    public CanvasDefinitionDto? Definition { get; set; }
    public TypeDefinitionPredicatesDto? Commands { get; set; }
    public TypeDefinitionPredicatesDto? DomainEvents { get; set; }
    public TypeDefinitionPredicatesDto? UbiquitousLanguage { get; set; }
}