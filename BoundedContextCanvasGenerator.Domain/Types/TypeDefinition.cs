namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeDefinition(TypeFullName Name, IEnumerable<TypeFullName> ImplementedInterfaces)
{
    public TypeType Type => TypeType.Class;
}

public enum TypeType
{
    Class,
    Interface
}