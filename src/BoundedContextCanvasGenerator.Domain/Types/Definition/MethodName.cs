using System.Diagnostics;

namespace BoundedContextCanvasGenerator.Domain.Types.Definition;

[DebuggerDisplay("{Value}")]
public record MethodName(string Value);