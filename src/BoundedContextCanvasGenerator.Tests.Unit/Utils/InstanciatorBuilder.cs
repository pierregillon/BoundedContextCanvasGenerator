using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public class InstanciatorBuilder
{
    private TypeDefinition _typeDefinition;
    private MethodInfo _methodInfo;

    public InstanciatorBuilder OfType(TypeDefinition typeDefinition)
    {
        _typeDefinition = typeDefinition;
        return this;
    }

    public InstanciatorBuilder FromMethod(MethodInfo methodInfo)
    {
        _methodInfo = methodInfo;
        return this;
    }

    public Instanciator Build() => new Instanciator(_typeDefinition, _methodInfo);

    public static implicit operator Instanciator(InstanciatorBuilder builder) => builder.Build();

}