namespace BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;

public record UbiquitousLanguage(IEnumerable<CoreConcept> Concepts)
{
    public static UbiquitousLanguage None => new(Enumerable.Empty<CoreConcept>());
    public bool IsNotEmpty => this != None;
}