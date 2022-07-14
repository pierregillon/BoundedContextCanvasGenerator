using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Tests.Unit;

public class TypeDefinitionBuilder
{
    private readonly TypeFullName _className;
    private readonly List<TypeFullName> _interfaces = new();

    private TypeDefinitionBuilder(string className) => _className = new TypeFullName(className);

    public static TypeDefinitionBuilder Class(string className) => new (className);

    public TypeDefinitionBuilder Implementing(string interfaceName)
    {
        this._interfaces.Add(new TypeFullName(interfaceName));
        return this;
    }

    public static implicit operator TypeDefinition(TypeDefinitionBuilder builder) => builder.Build();

    private TypeDefinition Build() => new(_className, _interfaces);
}