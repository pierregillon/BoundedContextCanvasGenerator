using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record DomainEvent(string FriendlyName, TypeFullName TypeFullName)
{
    public static DomainEvent FromType(TypeDefinition typeDefinition) 
        => new(typeDefinition.FullName.Name.ToReadableSentence(), typeDefinition.FullName);
}