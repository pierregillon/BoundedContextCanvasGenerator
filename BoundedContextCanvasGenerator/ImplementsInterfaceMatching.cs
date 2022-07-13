﻿using System.Text.RegularExpressions;
using LivingDocumentation.Domain;

namespace BoundedContextCanvasGenerator;

public class ImplementsInterfaceMatching : ITypeDefinitionPredicate
{
    private readonly Regex _regex;

    public ImplementsInterfaceMatching(string pattern) => _regex = new Regex(pattern, RegexOptions.Compiled);

    public bool IsMatching(TypeDefinition type) => type.ImplementedInterfaces.Any(x => _regex.IsMatch(x.Value));
}