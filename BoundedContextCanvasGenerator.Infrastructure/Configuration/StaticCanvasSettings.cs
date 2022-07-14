using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class StaticCanvasSettings : ICanvasSettings
{
    public StaticCanvasSettings(ConfigurationDto dto)
    {
        Commands = dto?.Commands?.Build() ?? TypeDefinitionPredicates.Empty();
        DomainEvents = dto?.DomainEvents?.Build() ?? TypeDefinitionPredicates.Empty();
    }

    public TypeDefinitionPredicates Commands { get; }
    public TypeDefinitionPredicates DomainEvents { get; }
}