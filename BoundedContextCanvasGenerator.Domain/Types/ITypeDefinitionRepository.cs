namespace BoundedContextCanvasGenerator.Domain.Types;

public interface ITypeDefinitionRepository
{
    IAsyncEnumerable<TypeDefinition> GetAll(SolutionName name);
}