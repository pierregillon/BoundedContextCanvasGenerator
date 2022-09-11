using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class UbiquitousLanguageBuilder
{
    private readonly List<CoreConcept> concepts = new();

    public UbiquitousLanguageBuilder WithCoreConcept(CoreConcept coreConcept)
    {
        concepts.Add(coreConcept);
        return this;
    }

    public UbiquitousLanguage Build() => new(concepts);

    public static implicit operator UbiquitousLanguage(UbiquitousLanguageBuilder builder) => builder.Build();
}