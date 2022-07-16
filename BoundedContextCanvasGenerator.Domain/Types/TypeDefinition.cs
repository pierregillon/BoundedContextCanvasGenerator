namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeDefinition(
    TypeFullName FullName, 
    TypeDescription Description, 
    TypeKind Kind,
    TypeModifiers Modifiers,
    IEnumerable<TypeFullName> ImplementedInterfaces
);