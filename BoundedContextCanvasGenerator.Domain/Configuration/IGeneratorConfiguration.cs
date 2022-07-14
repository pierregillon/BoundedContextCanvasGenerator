namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface IGeneratorConfiguration
{
    public IEnumerable<ITypeDefinitionPredicate> CommandDefinitions { get; }
}