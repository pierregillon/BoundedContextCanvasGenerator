using System.Collections.Generic;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using BoundedContextCanvasGenerator.Infrastructure.Types;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public class MethodDefinitionsTypeBuilder
{
    private readonly List<MethodDefinition> _methodDefinitions = new();

    public IReadOnlyCollection<MethodDefinition> MethodDefinitions => _methodDefinitions;
    public TypeFullName FullName { get; private set; }


    public MethodDefinitionsTypeBuilder Named(string value)
    {
        this.FullName = new TypeFullName(value);
        return this;
    }

    public MethodDefinitionsTypeBuilder WithMethod(MethodDefinition methodDefinition)
    {
        this._methodDefinitions.Add(methodDefinition);
        return this;
    }

}