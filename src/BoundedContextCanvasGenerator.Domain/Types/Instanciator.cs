using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Domain.Types;

public record Instanciator(TypeDefinition Type, IEnumerable<MethodInfo> Methods)
{
    public IEnumerable<MethodInfo> FilterMethodsMatching(IEnumerable<PolicyDefinition> policyDefinitions)
        => Methods.Where(method => policyDefinitions.Any(policy => policy.Match(method)));

    public IEnumerable<CollaboratorDefinition> FilterCollaboratorDefinitionsMatching(IEnumerable<CollaboratorDefinition> collaboratorDefinitions)
        => collaboratorDefinitions.Where(collaboratorDefinition => collaboratorDefinition.Match(Type));
}