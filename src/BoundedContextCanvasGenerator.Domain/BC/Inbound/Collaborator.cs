using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record Collaborator(string Name)
{
    public static Collaborator FromCollaboratorDefinition(CollaboratorDefinition collaboratorDefinition) => new(collaboratorDefinition.Name.ToReadableSentence());
}