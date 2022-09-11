using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Types;

public record ExtractedElements(bool IsEnabled, IReadOnlyCollection<TypeDefinition> Values);