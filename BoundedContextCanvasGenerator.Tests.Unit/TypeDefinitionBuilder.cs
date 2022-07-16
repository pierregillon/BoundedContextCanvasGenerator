using System;
using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Tests.Unit;

public class TypeDefinitionBuilder
{
    private readonly TypeKind _kind;
    private readonly TypeFullName _className;
    private readonly List<TypeFullName> _interfaces = new();
    private TypeDescription _description = TypeDescription.Empty;
    private TypeModifiers _modifiers = TypeModifiers.None;

    private TypeDefinitionBuilder(string className, TypeKind kind)
    {
        _kind = kind;
        _className = new TypeFullName(className);
    }

    public static TypeDefinitionBuilder Class(string className) => new TypeDefinitionBuilder(className, TypeKind.Class).Concrete();

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

    public TypeDefinitionBuilder Abstract()
    {
        if (this._modifiers.HasFlag(TypeModifiers.Concrete)) {
            this._modifiers = this._modifiers.RemoveFlag(TypeModifiers.Concrete);
        }
        this._modifiers = this._modifiers.AddFlag(TypeModifiers.Abstract);
        return this;
    }

    public TypeDefinitionBuilder Concrete()
    {
        if (this._modifiers.HasFlag(TypeModifiers.Abstract)) {
            this._modifiers = this._modifiers.RemoveFlag(TypeModifiers.Abstract);
        }
        this._modifiers = this._modifiers.AddFlag(TypeModifiers.Concrete);
        return this;
    }

    private TypeDefinition Build() => new(_className, _description, _kind, _modifiers, _interfaces);

    public static implicit operator TypeDefinition(TypeDefinitionBuilder builder) => builder.Build();
}