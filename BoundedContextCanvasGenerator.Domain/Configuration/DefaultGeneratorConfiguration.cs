using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;

namespace BoundedContextCanvasGenerator.Domain.Configuration;

public class DefaultGeneratorConfiguration : IGeneratorConfiguration
{
    public IEnumerable<ITypeDefinitionPredicate> CommandDefinitions
    {
        get {
            yield return new ImplementsInterfaceMatching(".*ICommand$");
        }
    }
}