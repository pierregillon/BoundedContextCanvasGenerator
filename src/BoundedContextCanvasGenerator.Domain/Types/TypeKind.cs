namespace BoundedContextCanvasGenerator.Domain.Types;

public enum TypeKind
{
    Class,
    Interface
}

public static class TypeKindExtensions
{
    public static TypeKind ToTypeKind(this string value) =>
        value switch
        {
            "class" => TypeKind.Class,
            "interface" => TypeKind.Interface,
            _ => throw new InvalidOperationException($"Unknown type name {value}")
        };
}