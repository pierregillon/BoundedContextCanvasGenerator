using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

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

public class StrategicClassificationDto
{
    public string? Domain { get; set; }
    public string? BusinessModel { get; set; }
    public string? Evolution { get; set; }

    public StrategicClassification Build() => new(Domain.ToDomainType(), BusinessModel.ToBusinessModel(), Evolution.ToEvolution());
}

public class DomainRoleDto
{
    public string? Name { get; set; }
    public string? Description{ get; set; }

    public DomainRole Build() => new(Text.From(Name), Text.From(Description));
}