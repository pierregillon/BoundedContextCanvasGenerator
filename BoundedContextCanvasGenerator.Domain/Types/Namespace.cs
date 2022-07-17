using System.Text;

namespace BoundedContextCanvasGenerator.Domain.Types;

public record Namespace(string Path)
{
    private const char SEPARATOR = '.';
    
    public static Namespace Empty => new(string.Empty);

    public static Namespace FromResourcePath(string fullPath)
    {
        var segments = fullPath.Split(SEPARATOR);
        return segments
            .Take(segments.Length - 1)
            .Pipe(FromSegments);
    }

    public static Namespace FromSegments(IEnumerable<string> segments) => new(segments.JoinWith(SEPARATOR));

    private IReadOnlyCollection<string> Segments { get; } = Path.Split(SEPARATOR);
    public string Name => this.Segments.Last();

    public IEnumerable<Namespace> GetSubNamespaces()
    {
        for (var i = 1; i <= this.Segments.Count; i++)
        {
            yield return this.Segments.Take(i).Pipe(FromSegments);
        }
    }

    public bool StartWith(Namespace start) 
        => start.Segments.Count <= this.Segments.Count && GetFirstDifferentSegmentIndex(start) == -1;

    private int GetFirstDifferentSegmentIndex(Namespace other)
    {
        for (var index = 0; index < other.Segments.Count; index++) {
            if (this.Segments.ElementAt(index) != other.Segments.ElementAt(index))
                return index;
        }

        return -1;
    }

    public virtual bool Equals(Namespace? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Path == other.Path;
    }

    public override int GetHashCode() => Path.GetHashCode();
}