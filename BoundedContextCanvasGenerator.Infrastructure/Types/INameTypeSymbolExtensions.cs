using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.CodeAnalysis;
using TypeKind = BoundedContextCanvasGenerator.Domain.Types.TypeKind;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public static class INameTypeSymbolExtensions
{
    public static TypeFullName GetFullName(this INamedTypeSymbol symbol) => new(symbol.ToString()!);

    public static TypeKind GetTypeKind(this INamedTypeSymbol symbol)
    {
        return symbol.TypeKind switch {
            Microsoft.CodeAnalysis.TypeKind.Class => TypeKind.Class,
            Microsoft.CodeAnalysis.TypeKind.Interface => TypeKind.Interface,
            _ => throw new InvalidOperationException($"Kind {symbol.TypeKind} not supported yet.")
        };
    }
}