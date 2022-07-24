using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public record Command(TypeDefinition TypeDefinition)
{
    public Namespace ParentNamespace { get; } = TypeDefinition.FullName.Namespace;
    public string FullName => TypeDefinition.FullName.Value;
    public string FriendlyName => TypeDefinition.FullName.Name.TrimWord("Command").ToReadableSentence();
    public Namespace Lane => ParentNamespace.TrimStart(TypeDefinition.AssemblyDefinition.Namespace);
    public string MermaidName => this.FullName;

    public IEnumerable<Namespace> GetSubNamespaces(bool splitIntoLanes)
    {
        return ParentNamespace
                .GetSubNamespaces()
                .Where(@namespace => !TypeDefinition.AssemblyDefinition.Namespace.StartWith(@namespace))
                .Where(@namespace => !splitIntoLanes || !@namespace.EndWith(Lane))
            ;
    }
}