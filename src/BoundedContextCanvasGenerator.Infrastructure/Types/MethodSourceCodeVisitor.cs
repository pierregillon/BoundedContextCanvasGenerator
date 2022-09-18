using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class MethodSourceCodeVisitor : CSharpSyntaxWalker
{
    private readonly MethodDefinitions _visitedData;
    private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModels;
    private readonly IDictionary<ISymbol, IEnumerable<ISymbol>> _alreadyParsedSymbols = new Dictionary<ISymbol, IEnumerable<ISymbol>>();

    public MethodSourceCodeVisitor(IEnumerable<SemanticModel> semanticModels, MethodDefinitions visitedData)
    {
        _visitedData = visitedData;
        _semanticModels = semanticModels.ToDictionary(x => x.SyntaxTree);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax methodNode)
    {
        if (methodNode.Parent is null) {
            base.VisitMethodDeclaration(methodNode);
            return;
        }

        var classType = GetSymbol(methodNode.Parent);
        if (classType is null) {
            base.VisitMethodDeclaration(methodNode);
            return;
        }

        var results = FindSymbolsInMethodDeclarationSyntax(methodNode).ToArray();
        if (results.Any()) {
            var methodDefinition = new MethodDefinition(methodNode.GetInfo(), results.Select(x => x.GetFullName()).ToArray());
            _visitedData.AddMethod(classType.GetFullName(), methodDefinition);
        }

        base.VisitMethodDeclaration(methodNode);
    }

    private IEnumerable<ISymbol> FindSymbolsInMethodDeclarationSyntax(BaseMethodDeclarationSyntax syntax)
    {
        if (syntax.ExpressionBody is not null) {
            return FindSymbolsInExpression(syntax.ExpressionBody.Expression);
        }

        return syntax.Parent is null || syntax.Body is null
            ? Enumerable.Empty<ISymbol>()
            : syntax.Body.Statements.SelectMany(FindSymbolsInStatement);
    }

    private IEnumerable<ISymbol> FindSymbolsInStatement(StatementSyntax statement) =>
        statement switch {
            IfStatementSyntax ifStatementSyntax => FindSymbolsInStatement(ifStatementSyntax.Statement),
            BlockSyntax blockSyntax => blockSyntax.Statements.SelectMany(FindSymbolsInStatement),
            LocalDeclarationStatementSyntax localDeclarationStatementSyntax => localDeclarationStatementSyntax
                .Declaration.Variables
                .Select(declaratorSyntax => declaratorSyntax.Initializer?.Value)
                .Where(syntax => syntax is not null)
                .Select(syntax => syntax!)
                .SelectMany(FindSymbolsInExpression),
            ExpressionStatementSyntax expressionStatementSyntax => FindSymbolsInExpression(expressionStatementSyntax.Expression),
            ReturnStatementSyntax { Expression: { } expression } => FindSymbolsInExpression(expression),
            _ => Enumerable.Empty<ISymbol>()
        };

    private IEnumerable<ISymbol> FindSymbolsInExpression(ExpressionSyntax syntax) =>
        syntax switch {
            ObjectCreationExpressionSyntax objectCreationExpressionSyntax => GetSymbols(objectCreationExpressionSyntax),
            InvocationExpressionSyntax invocationExpressionSyntax => GetSymbols(invocationExpressionSyntax),
            AwaitExpressionSyntax awaitExpressionSyntax => FindSymbolsInExpression(awaitExpressionSyntax.Expression),
            AssignmentExpressionSyntax assignmentExpressionSyntax => FindSymbolsInExpression(assignmentExpressionSyntax.Right),
            _ => Enumerable.Empty<ISymbol>()
        };

    private IEnumerable<ISymbol> GetSymbols(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        foreach (var argument in invocationExpressionSyntax.ArgumentList.Arguments) {
            foreach (var symbol1 in FindSymbolsInExpression(argument.Expression)) {
                yield return symbol1;
            }
        }

        var symbol = GetSymbol(invocationExpressionSyntax.Expression);

        if (symbol is not null) {
            foreach (var symbol1 in GetSymbolsFromDeclaration(symbol))
                yield return symbol1;
        }
    }

    private IEnumerable<ISymbol> GetSymbols(ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
    {
        var symbol = GetSymbol(objectCreationExpressionSyntax.Type);

        if (symbol is null || !symbol.DeclaringSyntaxReferences.Any()) yield break;

        yield return symbol;

        foreach (var symbol1 in GetSymbolsFromDeclaration(symbol))
            yield return symbol1;
    }

    private IEnumerable<ISymbol> GetSymbolsFromDeclaration(ISymbol symbol)
    {
        if (!_alreadyParsedSymbols.TryGetValue(symbol, out var instanciatedSymbols)) {
            instanciatedSymbols = TraverseDeclaringReferences(symbol).ToArray();
            _alreadyParsedSymbols.Add(symbol, instanciatedSymbols);
        }

        return instanciatedSymbols;
    }

    private IEnumerable<ISymbol> TraverseDeclaringReferences(ISymbol symbol)
    {
        foreach (var syntax in symbol.DeclaringSyntaxReferences.Select(reference => reference.GetSyntax())) {
            if (syntax is ClassDeclarationSyntax classDeclarationSyntax)
                foreach (var memberDeclarationSyntax in classDeclarationSyntax.Members) {
                    if (memberDeclarationSyntax is not ConstructorDeclarationSyntax constructorDeclarationSyntax) continue;

                    foreach (var aSymbol in FindSymbolsInMethodDeclarationSyntax(constructorDeclarationSyntax)) {
                        yield return aSymbol;
                    }
                }
            else if (syntax is MethodDeclarationSyntax methodDeclarationSyntax) {
                foreach (var result in FindSymbolsInMethodDeclarationSyntax(methodDeclarationSyntax)) {
                    yield return result;
                }
            }
        }
    }

    private ISymbol? GetSymbol(ExpressionSyntax expression)
    {
        try {
            return _semanticModels[expression.SyntaxTree].GetSymbolInfo(expression).Symbol;
        }
        catch (ArgumentException e) {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    private ISymbol? GetSymbol(SyntaxNode syntaxNode) => _semanticModels[syntaxNode.SyntaxTree].GetDeclaredSymbol(syntaxNode);
}