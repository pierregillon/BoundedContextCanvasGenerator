using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record Flowchart : IMermaidGeneratable
{
    private readonly Orientation? orientation;
    private ImmutableList<Node> Nodes { get; init; } = ImmutableList<Node>.Empty;
    private ImmutableList<Link> Links { get; init; } = ImmutableList<Link>.Empty;
    private ImmutableList<Subgraph> Subgraphs { get; init; } = ImmutableList<Subgraph>.Empty;
    private NodeStyleClass? DefaultNodeClass { get; init; }

    public static Flowchart Start()
    {
        return Start(null);
    }

    public static Flowchart Start(Orientation? orientation)
    {
        return new(orientation);
    }

    private Flowchart(Orientation? orientation)
    {
        this.orientation = orientation;
    }

    public Flowchart WithNode(Node node) =>
        this with
        {
            Nodes = this.Nodes.Add(node)
        };

    public Flowchart WithNodes(IEnumerable<Node> toAdd) =>
        this with
        {
            Nodes = this.Nodes.AddRange(toAdd)
        };

    public Flowchart Append(Flowchart other) =>
        this with
        {
            Nodes = this.Nodes.AddRange(other.Nodes),
            Links = this.Links.AddRange(other.Links),
            Subgraphs = this.Subgraphs.AddRange(other.Subgraphs),
        };

    public string ToMermaid() => this.ToMermaid(1);

    private IEnumerable<NodeStyleClass?> AllClassDefinitions =>
        this.Subgraphs.Select(subgraph => subgraph.StyleClass)
            .Concat(this.Nodes
                .Select(node => node.StyleClass))
            .Concat(this.Subgraphs
                .SelectMany(sub => sub.Content.AllClassDefinitions));

    public string ToMermaid(int indentation)
    {
        var classDefinitions = new List<IMermaidGeneratable>();

        if (indentation == 1)
        {
            classDefinitions =
                this.AllClassDefinitions
                    .Prepend(this.DefaultNodeClass)
                    .Where(styleClass => styleClass != null)
                    .GroupBy(styleClass => styleClass!.Name)
                    .Select(group => group.First()!)
                    .ToList<IMermaidGeneratable>();
        }

        var linkStyles = this.Links
            .Select((link, index) => link.LinkStyleToMermaid(indentation, index))
            .Where(declaration => declaration != null)
            .ToList();

        var contents = classDefinitions
            .Concat(this.Nodes)
            .Concat(this.Subgraphs)
            .Concat(this.Links)
            .Select(node => node.ToMermaid(indentation))
            .Concat(linkStyles);

        if (indentation == 1)
        {
            string header = "flowchart " + (this.orientation ?? Orientation.LeftToRight).ToMermaid();
            contents = contents.Prepend(header);
        }
        else if (this.orientation.HasValue)
        {
            string header = $"{Mermaid.Indent(indentation)}direction {this.orientation.Value.ToMermaid()}";
            contents = contents.Prepend(header);
        }

        return string.Join(Environment.NewLine, contents);
    }

    public Flowchart WithLink(Link link) =>
        this with
        {
            Links = this.Links.Add(link)
        };

    public Flowchart WithSubgraph(Subgraph subgraph) =>
        this with
        {
            Subgraphs = this.Subgraphs.Add(subgraph)
        };

    public Flowchart WithDefaultNodeStyle(NodeStyle defaultNodeStyle) =>
        this with
        {
            DefaultNodeClass = new NodeStyleClass("default", defaultNodeStyle)
        };
}