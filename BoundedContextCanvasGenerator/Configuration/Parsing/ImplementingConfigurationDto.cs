using BoundedContextCanvasGenerator;

public class ImplementingConfigurationDto
{
    public string? Pattern { get; set; }

    public ITypeDefinitionPredicate Build()
    {
        if (Pattern is null) {
            throw new InvalidOperationException("Pattern must be defined");
        }

        return new ImplementsInterfaceMatching(Pattern);
    }
}