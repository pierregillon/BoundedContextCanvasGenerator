using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public record ExtractedElements(bool IsEnabled, IReadOnlyCollection<TypeDefinition> Values)
{
    public int Count => Values.Count;
}