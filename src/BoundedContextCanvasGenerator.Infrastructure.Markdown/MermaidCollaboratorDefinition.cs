using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public record MermaidCollaboratorDefinition(string Name, TypeDefinitionPredicates Predicates)
{
    public string FriendlyName => this.Name.ToReadableSentence();
    public string MermaidName => this.Name + "Collaborator";

    public bool Match(TypeDefinition typeDefinition) => this.Predicates.AllMatching(typeDefinition);

    public string NameFor(Command command) => command.MermaidName + this.MermaidName;
}