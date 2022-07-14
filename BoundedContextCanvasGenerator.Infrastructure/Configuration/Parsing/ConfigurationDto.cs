namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class ConfigurationDto
{
    public string? Name { get; set; }
    public CanvasDefinitionDto? Definition { get; set; }
    public CommandConfigurationDto? Commands { get; set; }
    public DomainEventConfigurationDto? DomainEvents { get; set; }
}