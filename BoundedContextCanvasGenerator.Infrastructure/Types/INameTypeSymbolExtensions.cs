using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.CodeAnalysis;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public static class INameTypeSymbolExtensions
{
    public static TypeFullName GetFullName(this INamedTypeSymbol symbol) => new(symbol.ToString()!);
}