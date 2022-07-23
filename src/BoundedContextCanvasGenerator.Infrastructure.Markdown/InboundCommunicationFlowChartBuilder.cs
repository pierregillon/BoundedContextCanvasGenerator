using System.Text.RegularExpressions;
using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Configuration.Predicates;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;
using Grynwald.MarkdownGenerator;

namespace BoundedContextCanvasGenerator.Infrastructure.Markdown;

public class InboundCommunicationFlowChartBuilder
{
    private readonly IReadOnlyCollection<Command> _commands;

    public static InboundCommunicationFlowChartBuilder From(IReadOnlyCollection<TypeDefinition> types) => new(types.Select(x => new Command(x)).ToArray());

    public InboundCommunicationFlowChartBuilder(IReadOnlyCollection<Command> commands)
    {
        if (commands.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(commands));

        _commands = commands;
    }

    public MdContainerBlock Build(bool splitIntoLanes) => 
        !splitIntoLanes 
            ? new MdContainerBlock(GenerateAllCommands()) 
            : new MdContainerBlock(GenerateLanes());

    private MdCodeBlock GenerateAllCommands() => GenerateMermaidBlock(_commands, false);

    private IEnumerable<MdBlock> GenerateLanes()
    {
        return _commands
            .GroupBy(x => x.Lane)
            .SelectMany(x => new MdBlock[] {
                new MdHeading(3, x.Key.Name),
                new MdParagraph(new MdRawMarkdownSpan("---")),
                GenerateMermaidBlock(x, true)
            });
    }

    private static MdCodeBlock GenerateMermaidBlock(IEnumerable<Command> commands, bool splitIntoLanes)
    {
        var flowChart = Flowchart.Start(Orientation.LeftToRight);

        var root = Node
            .Named(new MermaidName("Collaborators", "WebApp"))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyle("fill:#f9f,stroke:#333,stroke-width:2px"));

        flowChart = flowChart.WithNode(root);
        flowChart = new Tree(root, splitIntoLanes)
            .GenerateNodes(commands)
            .Aggregate(flowChart, Merge);

        return new MermaidBlock(flowChart);
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

public record Command(TypeDefinition TypeDefinition)
{
    public Namespace ParentNamespace { get; } = TypeDefinition.FullName.Namespace;
    public string FullName => TypeDefinition.FullName.Value;
    public string FriendlyName => TypeDefinition.FullName.Name.TrimWord("Command").ToReadableSentence();
    public Namespace Lane => ParentNamespace.TrimStart(TypeDefinition.AssemblyDefinition.Namespace);
    public string MermaidName => this.FullName;

    public IEnumerable<Namespace> GetSubNamespaces(bool splitIntoLanes)
    {
        return ParentNamespace
            .GetSubNamespaces()
            .Where(@namespace => !TypeDefinition.AssemblyDefinition.Namespace.StartWith(@namespace))
            .Where(@namespace => !splitIntoLanes || !@namespace.EndWith(Lane))
            ;
    }
}

public class InboundCommunicationFlowChartBuilder2
{
    private readonly IEnumerable<CollaboratorDefinition> _collaboratorDefinitions;
    private readonly IEnumerable<PolicyDefinition> _policyDefinitions;

    public InboundCommunicationFlowChartBuilder2(IEnumerable<CollaboratorDefinition> collaboratorDefinitions, IEnumerable<PolicyDefinition> policyDefinitions)
    {
        _collaboratorDefinitions = collaboratorDefinitions;
        _policyDefinitions = policyDefinitions;
    }

    public MdContainerBlock Build(IReadOnlyCollection<TypeDefinition> typeDefinitions)
    {
        if (typeDefinitions.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(typeDefinitions));

        var commands = typeDefinitions.Select(x => new Command(x)).ToArray();

        return new MdContainerBlock(GenerateMermaidBlock(commands, false));
    }

    private MdCodeBlock GenerateMermaidBlock(IEnumerable<Command> commands, bool splitIntoLanes)
    {
        var flowChart = Flowchart.Start(Orientation.LeftToRight);

        //this.collaboratorNodes = _collaboratorDefinitions
        //    .Select(x => new { x.Name, Node = BuildNode(x) })
        //    .ToDictionary(x => x.Name, x => x.Node);

        //var root = Node
        //    .Named(new MermaidName("Collaborators", "WebApp"))
        //    .Shaped(NodeShape.Asymmetric)
        //    .Styled(new NodeStyle("fill:#f9f,stroke:#333,stroke-width:2px"));

        //flowChart = flowChart.WithNode(root);

        var allNodes = commands.SelectMany(GenerateCommandNodes);

        flowChart = allNodes.Aggregate(flowChart, Merge);

        return new MermaidBlock(flowChart);
    }

    private IEnumerable<IMermaidGeneratable> GenerateCommandNodes(Command command)
    {
        var node = BuildNode(command);

        yield return node;

        var collaboratorNodes = command.TypeDefinition.Instanciators
            .Select(x => new {
                Instanciator = x,
                Collaborators = _collaboratorDefinitions.Where(c => c.Match(x.Type))
            })
            .SelectMany(x => x.Collaborators.Select(c => BuildNode(c, command)))
            .ToArray();

        foreach (var collaboratorNode in collaboratorNodes) {
            yield return collaboratorNode;
            yield return Link.From(collaboratorNode).To(node);
        }

        var policies = command.TypeDefinition.Instanciators
            .Where(x => _policyDefinitions.Any(c => c.Match(x.Method)))
            .Select(x => x.Method.Name.Value.ToReadableSentence())
            .ToArray()
            .Pipe(Policies.From);

        if (policies.Any()) {
            var policyNode = BuildNode(policies, command);
            yield return policyNode;
            yield return Link.From(node).To(policyNode).WithOptions(LinkOptions.Default.WithHead(LinkHead.None));
        }
    }

    private static Node BuildNode(Command command)
        => Node.Named(new MermaidName(command.MermaidName, command.FriendlyName));

    private static Node BuildNode(CollaboratorDefinition collaborator, Command command)
        => Node
            .Named(new MermaidName(collaborator.NameFor(command), collaborator.FriendlyName))
            .Shaped(NodeShape.Asymmetric)
            .Styled(new NodeStyleClass("collaborators", new NodeStyle("fill:#FFE5FF")));    
    
    private static Node BuildNode(Policies policies, Command command)
        => Node
            .Named(new MermaidName(policies.NameFor(command), policies.FriendlyName))
            .Shaped(NodeShape.Parallelogram)
            .Styled(new NodeStyleClass("policies", new NodeStyle("fill:#FFFFAD, font-style:italic")));


    private static Flowchart Merge(Flowchart flowchart, IMermaidGeneratable element)
    {
        return element switch
        {
            Node node => flowchart.WithNode(node),
            Link link => flowchart.WithLink(link),
            _ => throw new NotImplementedException("Not supported element")
        };
    }
}

public record MermaidBlock(Flowchart Flowchart)
{
    private const string MERMAID_LANGUAGE = "mermaid";
    
    public MdCodeBlock ToCodeBlock() => new(Flowchart.ToMermaid(), MERMAID_LANGUAGE);

    public static implicit operator MdCodeBlock(MermaidBlock block) => block.ToCodeBlock();
}

public record CollaboratorDefinition(string Name, TypeDefinitionPredicates Predicates)
{
    public string FriendlyName => this.Name.ToReadableSentence();
    public string MermaidName => this.Name + "Collaborator";

    public bool Match(TypeDefinition typeDefinition) => this.Predicates.AllMatching(typeDefinition);

    public string NameFor(Command command) => command.MermaidName + this.MermaidName;
}

public record PolicyDefinition(MethodAttributeMatch Matcher)
{
    public bool Match(MethodInfo method) => method.Attributes.Any(x => Matcher.Match(x));
}

public record MethodAttributeMatch(string Pattern)
{
    private readonly Regex _regex = new(Pattern, RegexOptions.Compiled);

    public bool Match(MethodAttribute attribute)
        => _regex.IsMatch(attribute.Value);

    public virtual bool Equals(ImplementsInterfaceMatching? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Pattern == other.Pattern;
    }

    public override int GetHashCode() => Pattern.GetHashCode();
}

public record Policies(IEnumerable<string> Values)
{
    private const string MermaidName = "Policies";
    public string FriendlyName => Values.JoinWith("<br/>");

    public static Policies From(IEnumerable<string> values) => new(values);

    public bool Any() => this.Values.Any();

    public string NameFor(Command command) => command.MermaidName + MermaidName;
}