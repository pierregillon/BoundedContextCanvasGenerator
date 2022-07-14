namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class ConfigurationDto
{
    public CommandConfigurationDto? Commands { get; set; }
    public DomainEventConfigurationDto? DomainEvents { get; set; }
}