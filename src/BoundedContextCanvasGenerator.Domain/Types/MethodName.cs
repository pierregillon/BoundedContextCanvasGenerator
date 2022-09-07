using System.Diagnostics;

namespace BoundedContextCanvasGenerator.Domain.Types;

[DebuggerDisplay("{Value}")]
public record MethodName(string Value);