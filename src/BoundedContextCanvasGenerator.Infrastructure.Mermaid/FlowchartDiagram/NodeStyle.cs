using System.Collections.Generic;

namespace BoundedContextCanvasGenerator.Infrastructure.Mermaid.FlowchartDiagram;

public record NodeStyle(string Css)
{
    public static NodeStyleBuilder Start()
    {
        return new();
    }

    public static implicit operator NodeStyle(string value) => new(value);

    public record NodeStyleBuilder
    {
        private readonly List<string> styles = new();

        public NodeStyleBuilder Fill(string color) => this.AddStyle("fill", color);
        public NodeStyleBuilder Stroke(string color) => this.AddStyle("stroke", color);
        public NodeStyleBuilder StrokeWidth(string width) => this.AddStyle("stroke-width", width);
        public NodeStyleBuilder Color(string color) => this.AddStyle("color", color);
        public NodeStyleBuilder StrokeDashArray(params int[] lengths) => this.AddStyle("stroke-dasharray", string.Join(" ", lengths));

        public NodeStyle Build()
        {
            return new(string.Join(",", this.styles));
        }

        private NodeStyleBuilder AddStyle(string key, string value)
        {
            this.styles.Add($"{key}:{value}");
            return this;
        }

        public static implicit operator NodeStyle(NodeStyleBuilder value) => value.Build();
    }
}