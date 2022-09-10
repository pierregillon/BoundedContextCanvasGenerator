using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record Command(string Name, TypeFullName TypeFullName);