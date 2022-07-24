namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public static class A
{
    public static MethodDefinitionsBuilder MethodDefinitions => new();
    public static MethodDefinitionBuilder MethodDefinition => new();
    public static MethodDefinitionsTypeBuilder Type => new();
    public static TypeDefinitionBuilder Class(string name) => TypeDefinitionBuilder.Class(name);
    public static MethodInfoBuilder Method => new();
}