using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.BC;

public record ExtractedElements(bool IsEnabled, IReadOnlyCollection<TypeDefinition> Values)
{
    public int Count => Values.Count;
}