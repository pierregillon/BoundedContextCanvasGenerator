using BoundedContextCanvasGenerator.Domain.Types.Definition;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public static class MethodDeclarationSyntaxExtensions
{
    public static MethodInfo GetInfo(this MethodDeclarationSyntax syntax)
    {
        return new MethodInfo(
            new MethodName(syntax.Identifier.ToString()),
            syntax.AttributeLists
                .Select(x => new MethodAttribute(x.Attributes.ToFullString()))
                .ToArray()
        );
    }
}