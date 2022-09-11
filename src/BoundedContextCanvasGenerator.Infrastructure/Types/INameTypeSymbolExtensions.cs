using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types.Definition;
using Microsoft.CodeAnalysis;
using TypeKind = BoundedContextCanvasGenerator.Domain.Types.Definition.TypeKind;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public static class INameTypeSymbolExtensions
{
    public static TypeDefinition ToTypeDefinition(this ISymbol symbol)
    {
        return new TypeDefinition(
            symbol.GetFullName(),
            symbol.GetDescription(),
            symbol.GetTypeKind(),
            symbol.GetModifiers(),
            symbol.GetAllInterfaces(),
            symbol.GetAssemblyDefinition(),
            Array.Empty<Instanciator>()
        );
    }

    public static IEnumerable<TypeFullName> GetAllInterfaces(this ISymbol symbol)
    {
        if (symbol is not INamedTypeSymbol namedTypeSymbol) {
            return Array.Empty<TypeFullName>();
        }
        return Enumerable.Select(namedTypeSymbol.AllInterfaces, i => i.GetFullName()).ToArray();
    }

    public static TypeFullName GetFullName(this ISymbol symbol) => new(symbol.ToString()!);

    private static TypeDescription GetDescription(this ISymbol symbol)
    {
        var xml = symbol.GetDocumentationCommentXml();
        return string.IsNullOrWhiteSpace(xml)
            ? TypeDescription.Empty
            : new DocumentationComment(xml).GetSummary().Pipe(TypeDescription.From);
    }

    private static TypeKind GetTypeKind(this ISymbol symbol)
    {
        if (symbol is not INamedTypeSymbol namedTypeSymbol) {
            return TypeKind.Unknown;
        }
        return namedTypeSymbol.TypeKind switch {
            Microsoft.CodeAnalysis.TypeKind.Class => TypeKind.Class,
            Microsoft.CodeAnalysis.TypeKind.Interface => TypeKind.Interface,
            _ => throw new InvalidOperationException($"Kind {namedTypeSymbol.TypeKind} not supported yet.")
        };
    }

    private static TypeModifiers GetModifiers(this ISymbol symbol)
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
    
    private static AssemblyDefinition GetAssemblyDefinition(this ISymbol symbol) 
        => new(new Namespace(symbol.ContainingAssembly.Name));
}