using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;

public record CoreConcept(string Name, string Description)
{
    public static CoreConcept FromTypeDefinition(TypeDefinition typeDefinition) 
        => new(typeDefinition.FullName.Name.ToReadableSentence(), typeDefinition.Description.Value);
}