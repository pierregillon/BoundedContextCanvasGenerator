namespace LivingDocumentation.BoundedContextCanvas.Domain;

public interface ITypeDefinitionRepository
{
    IAsyncEnumerable<TypeDefinition> GetAll(SolutionName name);
}