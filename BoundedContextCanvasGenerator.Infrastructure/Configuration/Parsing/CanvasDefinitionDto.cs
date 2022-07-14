using BoundedContextCanvasGenerator.Domain.Configuration;
using YamlDotNet.Serialization;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing;

public class CanvasDefinitionDto
{
    public string? Description { get; set; }
    
    public StrategicClassificationDto? StrategicClassification { get; set; }

    public DomainRoleDto? DomainRole { get; set; }

    public CanvasDefinition Build() => new(
        Text.From(Description),
        this.StrategicClassification?.Build() ?? Domain.Configuration.StrategicClassification.Empty,
        this.DomainRole?.Build() ?? Domain.Configuration.DomainRole.Empty
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

    public DomainRole Build() => new DomainRole(Text.From(Name), Text.From(Description));
}