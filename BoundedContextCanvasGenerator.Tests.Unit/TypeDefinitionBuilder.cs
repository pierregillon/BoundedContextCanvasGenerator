using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Tests.Unit;

public class TypeDefinitionBuilder
{
    private readonly TypeKind _kind;
    private readonly TypeFullName _className;
    private readonly List<TypeFullName> _interfaces = new();
    private TypeDescription _description = TypeDescription.Empty;

    private TypeDefinitionBuilder(string className, TypeKind kind)
    {
        _kind = kind;
        _className = new TypeFullName(className);
    }

    public static TypeDefinitionBuilder Class(string className) => new (className, TypeKind.Class);

    public TypeDefinitionBuilder WithDescription(string value)
    {
        _description = new TypeDescription(value);
        return this;
    }

    public TypeDefinitionBuilder Implementing(string interfaceName)
    {
        this._interfaces.Add(new TypeFullName(interfaceName));
        return this;
    }

    public static implicit operator TypeDefinition(TypeDefinitionBuilder builder) => builder.Build();

    private TypeDefinition Build() => new(_className, _description, _kind, _interfaces);
}