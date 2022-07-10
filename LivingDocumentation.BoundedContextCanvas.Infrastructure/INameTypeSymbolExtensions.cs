using LivingDocumentation.BoundedContextCanvas.Domain;
using Microsoft.CodeAnalysis;

namespace LivingDocumentation.BoundedContextCanvas.Infrastructure;

public static class INameTypeSymbolExtensions
{
    public static TypeFullName GetFullName(this INamedTypeSymbol symbol) => new(symbol.ToString()!);
}