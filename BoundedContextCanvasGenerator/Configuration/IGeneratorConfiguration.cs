namespace BoundedContextCanvasGenerator;

public interface IGeneratorConfiguration
{
    public IEnumerable<ITypeDefinitionPredicate> CommandDefinitions { get; }
}