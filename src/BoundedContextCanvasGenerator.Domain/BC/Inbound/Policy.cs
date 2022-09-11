using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record Policy(string Description)
{
    public static Policy FromMethod(MethodInfo methodInfo) => new(methodInfo.Name.Value.ToReadableSentence());
}