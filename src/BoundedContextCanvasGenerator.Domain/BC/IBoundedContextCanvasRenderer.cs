using System.Text;

namespace BoundedContextCanvasGenerator.Domain.BC;

public interface IBoundedContextCanvasRenderer
{
    Task<Bytes> Render(BoundedContextCanvas boundedContextCanvas);
}

public record Bytes(byte[] Content)
{
    public static Bytes FromString(string value) => new(Encoding.UTF8.GetBytes(value));
    public int Length => Content.Length;
    public override string ToString() => Encoding.UTF8.GetString(Content);
}