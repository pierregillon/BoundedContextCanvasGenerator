namespace BoundedContextCanvasGenerator.Domain.BC.Definition;

public record DomainRole(Text Title, Text Description)
{
    public static DomainRole Empty => new(Text.Empty, Text.Empty);
    public bool IsEmpty => this == Empty;
}