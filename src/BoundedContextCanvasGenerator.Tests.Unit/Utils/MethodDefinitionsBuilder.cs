using BoundedContextCanvasGenerator.Infrastructure.Types;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public class MethodDefinitionsBuilder
{
    private MethodDefinitions _methodDefinitions = new();
    public static MethodDefinitionsBuilder New() => new();

    public MethodDefinitionsBuilder With(MethodDefinitionsTypeBuilder type)
    {
        foreach (var typeMethodDefinition in type.MethodDefinitions) {
            _methodDefinitions.AddMethod(type.FullName, typeMethodDefinition);
        }
        return this;
    }

    public MethodDefinitions Build() => _methodDefinitions;
    public MethodDefinitions Empty() => new();

    public static implicit operator MethodDefinitions(MethodDefinitionsBuilder builder) => builder.Build();
}