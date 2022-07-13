using BoundedContextCanvasGenerator;

public class DefaultGeneratorConfiguration : IGeneratorConfiguration
{
    public IEnumerable<ITypeDefinitionPredicate> CommandDefinitions
    {
        get {
            yield return new ImplementsInterfaceMatching(".*ICommand$");
        }
    }
}