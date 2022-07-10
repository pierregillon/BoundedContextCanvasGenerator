namespace LivingDocumentation.BoundedContextCanvas.Domain;

public record TypeDefinition(TypeFullName Name, IEnumerable<TypeFullName> ImplementedInterfaces);