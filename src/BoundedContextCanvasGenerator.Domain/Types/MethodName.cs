namespace BoundedContextCanvasGenerator.Domain.Types;

public record MethodName(string Value);

public record MethodInfo(MethodName Name, IEnumerable<MethodAttribute> Attributes);

public record MethodAttribute(string Value);