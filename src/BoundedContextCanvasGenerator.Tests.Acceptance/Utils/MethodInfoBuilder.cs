using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

public class MethodInfoBuilder
{
    private MethodName _name;
    private readonly List<MethodAttribute> _attributes = new();

    public MethodInfoBuilder Named(string name)
    {
        this._name = new MethodName(name);
        return this;
    }

    public MethodInfoBuilder WithAttribute(string attribute)
    {
        this._attributes.Add(new MethodAttribute(attribute));
        return this;
    }

    public MethodInfo Build() => new(this._name, this._attributes);

    public static implicit operator MethodInfo(MethodInfoBuilder builder) => builder.Build();
}