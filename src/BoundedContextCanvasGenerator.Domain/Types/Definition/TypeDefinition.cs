namespace BoundedContextCanvasGenerator.Domain.Types.Definition;

public record TypeDefinition(
    TypeFullName FullName, 
    TypeDescription Description, 
    TypeKind Kind,
    TypeModifiers Modifiers,
    IEnumerable<TypeFullName> ImplementedInterfaces,
    AssemblyDefinition AssemblyDefinition,
    IEnumerable<Instanciator> Instanciators);