namespace BoundedContextCanvasGenerator.Domain.Types;

public record TypeDefinition(TypeFullName Name, TypeKind Type, IEnumerable<TypeFullName> ImplementedInterfaces);