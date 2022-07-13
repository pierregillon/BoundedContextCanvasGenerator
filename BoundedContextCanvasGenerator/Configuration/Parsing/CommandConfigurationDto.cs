using BoundedContextCanvasGenerator;

public class CommandConfigurationDto
{
    public string? Type { get; set; }
    public ImplementingConfigurationDto? Implementing { get; set; }

    public IEnumerable<ITypeDefinitionPredicate> Build()
    {
        if (Type is not null) {
            yield return new OfType(Type);
        }
        if (Implementing is not null) {
            yield return Implementing.Build();
        }
    }
}