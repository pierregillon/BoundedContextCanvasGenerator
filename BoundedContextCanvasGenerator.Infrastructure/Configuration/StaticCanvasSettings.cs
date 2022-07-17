﻿using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration;

public class StaticCanvasSettings : ICanvasSettings
{
    public StaticCanvasSettings(ConfigurationDto? dto)
    {
        Name = CanvasName.From(dto?.Name);
        Definition = dto?.Definition?.Build() ?? CanvasDefinition.Empty;
        UbiquitousLanguage = dto?.UbiquitousLanguage?.Build() ?? TypeDefinitionPredicates.Empty;
        Commands = dto?.InboundCommunication?.Build() ?? TypeDefinitionPredicates.Empty;
        DomainEvents = dto?.DomainEvents?.Build() ?? TypeDefinitionPredicates.Empty;
    }

    public CanvasName Name { get; }
    public CanvasDefinition Definition { get; }
    public TypeDefinitionPredicates Commands { get; }
    public TypeDefinitionPredicates DomainEvents { get; }
    public TypeDefinitionPredicates UbiquitousLanguage { get; }
}