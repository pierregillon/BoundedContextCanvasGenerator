﻿namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public record DomainEventDefinitionDto : ISelectable
{
    public string? Selector { get; set; }
    public HandlerDefinitionDto? Handler { get; set; }
}