public interface ITypeDefinitionRepository
{
    IAsyncEnumerable<TypeDefinition> GetAll(SolutionName name);
}