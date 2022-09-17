using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class StaticCanvasSettings : ICanvasSettings
{
    public StaticCanvasSettings(ConfigurationDto? dto)
    {
        Name = CanvasName.From(dto?.Name);
        Definition = dto?.Definition?.Build() ?? CanvasDefinition.Empty;
        UbiquitousLanguage = dto?.UbiquitousLanguage?.Build() ?? TypeDefinitionPredicates.Empty;
        InboundCommunicationSettings = dto?.InboundCommunication?.Build() ?? InboundCommunicationSettings.Empty;
        DomainEvents = dto?.DomainEvents?.Build() ?? TypeDefinitionPredicates.Empty;
    }

    public CanvasName Name { get; }
    public CanvasDefinition Definition { get; }
    public InboundCommunicationSettings InboundCommunicationSettings { get; }
    public TypeDefinitionPredicates DomainEvents { get; }
    public TypeDefinitionPredicates UbiquitousLanguage { get; }
}