using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Infrastructure.Types;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public class MethodDefinitionBuilder
{
    private List<TypeFullName> instanciatedTypes = new();
    private MethodInfo _methodInfo;

    public static MethodDefinitionBuilder New() => new();

    public MethodDefinitionBuilder WithInfo(MethodInfo methodInfo)
    {
        this._methodInfo = methodInfo;
        return this;
    }

    public MethodDefinitionBuilder Instanciating(string name)
    {
        this.instanciatedTypes.Add(new TypeFullName(name));
        return this;
    }

    public MethodDefinition Build() => new(this._methodInfo, this.instanciatedTypes);

    public static implicit operator MethodDefinition(MethodDefinitionBuilder builder) => builder.Build();
}