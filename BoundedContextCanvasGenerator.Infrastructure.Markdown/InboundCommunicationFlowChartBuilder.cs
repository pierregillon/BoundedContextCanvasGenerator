using System.Collections;
using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class InboundCommunicationFlowChartBuilder
{
    private const string MERMAID_LANGUAGE = "mermaid";

    private readonly IReadOnlyCollection<Command> _commands;

    public static InboundCommunicationFlowChartBuilder From(IReadOnlyCollection<TypeDefinition> types) => new(types.Select(x => new Command(x)).ToArray());

    public InboundCommunicationFlowChartBuilder(IReadOnlyCollection<Command> commands)
    {
        if (commands.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(commands));

        _commands = commands;
    }

    public MdContainerBlock Build()
    {
        var flowChart = Flowchart.Start(Orientation.LeftToRight);

        var root = Node
            .Named(new MermaidName("Collaborators", "WebApp"))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyle("fill:#f9f,stroke:#333,stroke-width:2px"));

        flowChart = flowChart.WithNode(root);
        flowChart = new Tree(root)
            .GenerateNodes(_commands)
            .Aggregate(flowChart, Merge);

        return new MdContainerBlock(new MdCodeBlock(flowChart.ToMermaid(), MERMAID_LANGUAGE));
    }

    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }

}

public record Tree(Node Root)
{
    private readonly IDictionary<Folder, Node> _alreadyCreatedFolderNodes = new Dictionary<Folder, Node>();

    public IEnumerable<IMermaidGeneratable> GenerateNodes(IReadOnlyCollection<Command> commands) => commands.SelectMany(GenerateNodes);

    private IEnumerable<IMermaidGeneratable> GenerateNodes(Command command)
    {
        var folderNodes = GenerateFolderNodes(command).ToArray();
        var commandNodes = GenerateCommandNodes(command).ToArray();
        return folderNodes.Concat(commandNodes);
    }

    private IEnumerable<IMermaidGeneratable> GenerateFolderNodes(Command command)
    {
        Node? previousSubFolderNode = null;
        foreach (var subFolder in command.ParentFolder.GetSubfolders())
        {
            if (this.TryCreateFolderNode(subFolder, out var folderNode))
            {
                yield return folderNode;
                yield return Link.From(previousSubFolderNode ?? Root).To(folderNode);
            }
            previousSubFolderNode = folderNode;
        }
    }

    private IEnumerable<IMermaidGeneratable> GenerateCommandNodes(Command command)
    {
        var node = BuildNode(command);
        yield return node;
        if (_alreadyCreatedFolderNodes.TryGetValue(command.ParentFolder, out var parentFolderNode)) {
            yield return Link.From(parentFolderNode).To(node);
        }
    }

    private bool TryCreateFolderNode(Folder folder, out Node result)
    {
        if (_alreadyCreatedFolderNodes.TryGetValue(folder, out var node))
        {
            result = node;
            return false;
        }
        var folderNode = BuildNode(folder);
        _alreadyCreatedFolderNodes.Add(folder, folderNode);
        result = folderNode;
        return true;
    }

    private static Node BuildNode(Folder folder) 
        => Node.Named(new MermaidName(folder.Path, folder.Name));

    private static Node BuildNode(Command command)
        => Node.Named(new MermaidName(command.FullName, command.FriendlyName));
}

public record Folder(string Path)
{
    private const char FolderSeparator = '.';

    public string[] Segments { get; } = Path.Split(FolderSeparator);
    public string Name => this.Segments.Last();

    public IEnumerable<Folder> GetSubfolders()
    {
        var root = string.Empty;
        foreach (var segment in Segments) {
            root = string.IsNullOrEmpty(root)
                ? segment
                : string.Join(FolderSeparator, root, segment);

            yield return new Folder(root);
        }
    }

    public virtual bool Equals(Folder? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Path == other.Path;
    }

    public override int GetHashCode()
    {
        return this.Path.GetHashCode();
    }

    public override string ToString() => this.Path;

}

public record Command(TypeDefinition TypeDefinition)
{
    public Folder ParentFolder { get; } = new(TypeDefinition.FullName.Namespace);
    public string FullName => TypeDefinition.FullName.Value;
    public string FriendlyName => TypeDefinition.FullName.Name.TrimWord("Command").ToReadableSentence();
}