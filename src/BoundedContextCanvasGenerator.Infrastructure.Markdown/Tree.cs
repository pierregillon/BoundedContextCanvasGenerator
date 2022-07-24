using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public record Tree(Node Root, bool splitIntoLanes)
{
    private readonly IDictionary<Namespace, Node> _alreadyCreatedNamespaceNodes = new Dictionary<Namespace, Node>();

    public IEnumerable<IMermaidGeneratable> GenerateNodes(IEnumerable<Command> commands) => commands.SelectMany(GenerateNodes);

    private IEnumerable<IMermaidGeneratable> GenerateNodes(Command command)
    {
        var namespaceNodes = GenerateNamespaceNodes(command).ToArray();
        var commandNodes = GenerateCommandNodes(command).ToArray();
        return namespaceNodes.Concat(commandNodes);
    }

    private IEnumerable<IMermaidGeneratable> GenerateNamespaceNodes(Command command)
    {
        Node? previousNamespaceNode = null;
        foreach (var subNamespace in command.GetSubNamespaces(splitIntoLanes))
        {
            if (this.TryCreateNamespaceNode(subNamespace, out var namespaceNode))
            {
                yield return namespaceNode;
                yield return Link.From(previousNamespaceNode ?? Root).To(namespaceNode);
            }
            previousNamespaceNode = namespaceNode;
        }
    }

    private IEnumerable<IMermaidGeneratable> GenerateCommandNodes(Command command)
    {
        var node = BuildNode(command);
        yield return node;
        if (_alreadyCreatedNamespaceNodes.TryGetValue(command.ParentNamespace, out var parentNamespaceNode)) {
            yield return Link.From(parentNamespaceNode).To(node);
        }
        else {
            yield return Link.From(Root).To(node);
        }
    }

    private bool TryCreateNamespaceNode(Namespace @namespace, out Node result)
    {
        if (_alreadyCreatedNamespaceNodes.TryGetValue(@namespace, out var node))
        {
            result = node;
            return false;
        }
        var namespaceNode = BuildNode(@namespace);
        _alreadyCreatedNamespaceNodes.Add(@namespace, namespaceNode);
        result = namespaceNode;
        return true;
    }

    private static Node BuildNode(Namespace @namespace) 
        => Node.Named(new MermaidName(@namespace.Path, @namespace.Name));

    private static Node BuildNode(Command command)
        => Node.Named(new MermaidName(command.FullName, command.FriendlyName));
}