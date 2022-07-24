using System;
using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types;
using AssemblyDefinition = BoundedContextCanvasGenerator.Domain.Types.AssemblyDefinition;
using TypeDefinition = BoundedContextCanvasGenerator.Domain.Types.TypeDefinition;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public class TypeDefinitionBuilder
{
    private readonly TypeKind _kind;
    private readonly TypeFullName _className;
    private readonly List<TypeFullName> _interfaces = new();
    private TypeDescription _description = TypeDescription.Empty;
    private TypeModifiers _modifiers = TypeModifiers.None;
    private AssemblyDefinition _assemblyDefinition = new(new Namespace("Some"));
    private readonly List<Instanciator> _instanciators = new();

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

    public TypeDefinitionBuilder InAssembly(string assemblyNamespace)
    {
        this._assemblyDefinition = new AssemblyDefinition(new Namespace(assemblyNamespace));
        return this;
    }

    public TypeDefinitionBuilder InstanciatedBy(Instanciator instanciator)
    {
        this._instanciators.Add(instanciator);

        return this;
    }    
    
    public TypeDefinitionBuilder InstanciatedBy(MethodInfo methodInfo, TypeDefinition typeDefinition)
    {
        this._instanciators.Add(new Instanciator(typeDefinition, methodInfo));

        return this;
    }

    private TypeDefinition Build() =>
        new(_className,
            _description,
            _kind,
            _modifiers,
            _interfaces,
            _assemblyDefinition,
            _instanciators
        );

    public static implicit operator TypeDefinition(TypeDefinitionBuilder builder) => builder.Build();
}