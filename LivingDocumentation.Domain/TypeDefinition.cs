namespace LivingDocumentation.Domain;

public record TypeDefinition(TypeFullName Name, IEnumerable<TypeFullName> ImplementedInterfaces);