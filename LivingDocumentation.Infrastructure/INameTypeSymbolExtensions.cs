using LivingDocumentation.Domain;
using Microsoft.CodeAnalysis;

namespace LivingDocumentation.Infrastructure;

public static class INameTypeSymbolExtensions
{
    public static TypeFullName GetFullName(this INamedTypeSymbol symbol) => new(symbol.ToString()!);
}