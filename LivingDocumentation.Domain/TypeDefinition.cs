namespace LivingDocumentation.Domain;

public record TypeDefinition(TypeFullName Name, IEnumerable<TypeFullName> ImplementedInterfaces)
{
    public TypeType Type => TypeType.Class;
}

public enum TypeType
{
    Class,
    Interface
}