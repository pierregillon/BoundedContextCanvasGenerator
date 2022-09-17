using BoundedContextCanvasGenerator.Domain.BC.Definition;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public class StrategicClassificationDto
{
    public string? Domain { get; set; }
    public string? BusinessModel { get; set; }
    public string? Evolution { get; set; }

    public StrategicClassification Build() => new(Domain.ToDomainType(), BusinessModel.ToBusinessModel(), Evolution.ToEvolution());
}