namespace BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;

public record UbiquitousLanguage(IEnumerable<CoreConcept> Concepts)
{
    public static UbiquitousLanguage FromConcepts(IEnumerable<CoreConcept> concepts) => new(concepts);
    public static UbiquitousLanguage None => new(Enumerable.Empty<CoreConcept>());
    public bool IsNotEmpty => this != None;

    public virtual bool Equals(UbiquitousLanguage? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Concepts.SequenceEqual(other.Concepts);
    }

    public override int GetHashCode() => Concepts.GetHashCode();
}