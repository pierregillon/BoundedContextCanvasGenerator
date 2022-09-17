using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public class CanvasDefinitionDto
{
    public string? Description { get; set; }
    
    public StrategicClassificationDto? StrategicClassification { get; set; }

    public DomainRoleDto? DomainRole { get; set; }

    public CanvasDefinition Build() => new(
        Text.From(Description),
        this.StrategicClassification?.Build() ?? Domain.BC.Definition.StrategicClassification.Empty,
        this.DomainRole?.Build() ?? Domain.BC.Definition.DomainRole.Empty
    );
}