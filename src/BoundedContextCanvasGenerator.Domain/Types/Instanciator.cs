using BoundedContextCanvasGenerator.Infrastructure.Markdown;

namespace BoundedContextCanvasGenerator.Domain.Types;

public record Instanciator(TypeDefinition Type, IEnumerable<MethodInfo> Methods)
{
    public IEnumerable<MethodInfo> GetMethodsMatching(IEnumerable<PolicyDefinition> policyDefinitions) 
        => this.Methods.Where(method => policyDefinitions.Any(policy => policy.Match(method)));
}