using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record Collaborator(string Name, CollaboratorType Type)
{
    public static Collaborator FromCollaboratorDefinition(CollaboratorDefinition collaboratorDefinition) 
        => new(collaboratorDefinition.Name.ToReadableSentence(), collaboratorDefinition.Type);
}

public enum CollaboratorType
{
    Front,
    BoundedContext
}