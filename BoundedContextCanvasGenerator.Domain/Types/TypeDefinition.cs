namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeDefinition(
    TypeFullName Name, 
    TypeDescription Description, 
    TypeKind Type, 
    IEnumerable<TypeFullName> ImplementedInterfaces
);