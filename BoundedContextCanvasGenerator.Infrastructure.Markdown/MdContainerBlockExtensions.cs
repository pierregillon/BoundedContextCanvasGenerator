using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public static class MdContainerBlockExtensions
{
    public static void AddRange(this MdContainerBlock container, IEnumerable<MdBlock> blocks)
    {
        foreach (var block in blocks) {
            container.Add(block);
        }
    }

    public static MdContainerBlock ToContainerBlock(this IEnumerable<MdBlock> blocks) => new(blocks);
}