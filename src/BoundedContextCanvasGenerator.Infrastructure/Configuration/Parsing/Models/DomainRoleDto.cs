using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;

namespace BoundedContextCanvasGenerator.Infrastructure.Configuration.Parsing.Models;

public class DomainRoleDto
{
    public string? Name { get; set; }
    public string? Description{ get; set; }

    public DomainRole Build() => new(Text.From(Name), Text.From(Description));
}