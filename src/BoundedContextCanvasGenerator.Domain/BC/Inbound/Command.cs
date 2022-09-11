using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record Command(string FriendlyName, TypeFullName TypeFullName);