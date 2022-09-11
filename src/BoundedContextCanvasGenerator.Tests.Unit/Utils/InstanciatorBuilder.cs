using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public class InstanciatorBuilder
{
    private TypeDefinition _typeDefinition;
    private List<MethodInfo> _methodInfos = new List<MethodInfo>();

    public InstanciatorBuilder OfType(TypeDefinition typeDefinition)
    {
        _typeDefinition = typeDefinition;
        return this;
    }

    public InstanciatorBuilder FromMethod(MethodInfo methodInfo)
    {
        _methodInfos.Add(methodInfo);
        return this;
    }

    public Instanciator Build() => new(_typeDefinition, _methodInfos);

    public static implicit operator Instanciator(InstanciatorBuilder builder) => builder.Build();

}