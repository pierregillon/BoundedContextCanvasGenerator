using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.CodeAnalysis;
using TypeKind = BoundedContextCanvasGenerator.Domain.Types.TypeKind;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public static class INameTypeSymbolExtensions
{
    public static TypeDefinition ToTypeDefinition(this INamedTypeSymbol symbol)
    {
        return new TypeDefinition(
            symbol.GetFullName(),
            symbol.GetDescription(),
            symbol.GetTypeKind(),
            symbol.GetModifiers(),
            Enumerable.Select(symbol.AllInterfaces, i => i.GetFullName()).ToArray()
        );
    }

    private static TypeFullName GetFullName(this INamedTypeSymbol symbol) => new(symbol.ToString()!);

    private static TypeDescription GetDescription(this INamedTypeSymbol symbol)
    {
        var xml = symbol.GetDocumentationCommentXml();
        return string.IsNullOrWhiteSpace(xml)
            ? TypeDescription.Empty
            : new DocumentationComment(xml).GetSummary().Pipe(TypeDescription.From);
    }

    private static TypeKind GetTypeKind(this INamedTypeSymbol symbol)
    {
        return symbol.TypeKind switch {
            Microsoft.CodeAnalysis.TypeKind.Class => TypeKind.Class,
            Microsoft.CodeAnalysis.TypeKind.Interface => TypeKind.Interface,
            _ => throw new InvalidOperationException($"Kind {symbol.TypeKind} not supported yet.")
        };
    }

    private static TypeModifiers GetModifiers(this INamedTypeSymbol symbol)
    {
        IEnumerable<TypeModifiers> Find()
        {
            if (symbol.IsAbstract) {
                yield return TypeModifiers.Abstract;
            }
            else {
                yield return TypeModifiers.Concrete;
            }
        }

        return Find().Aggregate();
    }
}