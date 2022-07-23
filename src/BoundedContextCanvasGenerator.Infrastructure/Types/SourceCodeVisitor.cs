using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class SourceCodeVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    private readonly VisitedData _visitedData;

    public SourceCodeVisitor(SemanticModel semanticModel, VisitedData visitedData)
    {
        _semanticModel = semanticModel;
        _visitedData = visitedData;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        AddVisitedType(node);

        base.VisitClassDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        AddVisitedType(node);

        base.VisitRecordDeclaration(node);
    }

    private void AddVisitedType(BaseTypeDeclarationSyntax node)
    {
        var type = _semanticModel.GetDeclaredSymbol(node);

        if (type != null) {
            _visitedData.AddTypeDefinition(type.ToTypeDefinition());
        }
    }
}

public class SourceCodeMethodVisitor : CSharpSyntaxWalker
{
    private readonly VisitedData2 _visitedData;
    private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModels;

    public SourceCodeMethodVisitor(IEnumerable<SemanticModel> semanticModels, VisitedData2 visitedData)
    {
        _visitedData = visitedData;
        _semanticModels = semanticModels.ToDictionary(x => x.SyntaxTree);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax methodNode)
    {
        if (methodNode.Parent is null || methodNode.Body is null)
        {
            base.VisitMethodDeclaration(methodNode);
            return;
        }

        var classType = GetSymbol(methodNode.Parent);
        if (classType is null)
        {
            base.VisitMethodDeclaration(methodNode);
            return;
        }

        var results = FindInstanciatedSymbolsFromMethodBody(methodNode).ToArray();
        if (results.Any())
        {
            var methodDefinition = new MethodDefinition(methodNode.GetInfo(), results.Select(x => x.GetFullName()).ToArray());
            _visitedData.AddMethod(classType.GetFullName(), methodDefinition);
        }

        base.VisitMethodDeclaration(methodNode);
    }

    private IEnumerable<ISymbol> FindInstanciatedSymbolsFromMethodBody(BaseMethodDeclarationSyntax methodNode)
        => methodNode.Parent is null || methodNode.Body is null
            ? Enumerable.Empty<ISymbol>()
            : methodNode.Body.Statements.SelectMany(FindSymbolsInStatement);

    private IEnumerable<ISymbol> FindSymbolsInStatement(StatementSyntax statement) =>
        statement switch
        {
            IfStatementSyntax ifStatementSyntax => FindSymbolsInStatement(ifStatementSyntax.Statement),
            BlockSyntax blockSyntax => blockSyntax.Statements.SelectMany(FindSymbolsInStatement),
            LocalDeclarationStatementSyntax localDeclarationStatementSyntax => GetSymbols(localDeclarationStatementSyntax),
            ExpressionStatementSyntax expressionStatementSyntax => GetSymbols(expressionStatementSyntax),
            ReturnStatementSyntax { Expression: ObjectCreationExpressionSyntax objectCreationExpressionSyntax } => GetSymbols(objectCreationExpressionSyntax),
            _ => Enumerable.Empty<ISymbol>()
        };

    private IEnumerable<ISymbol> GetSymbols(LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
    {
        foreach (var variableDeclaratorSyntax in localDeclarationStatementSyntax.Declaration.Variables)
        {
            if (variableDeclaratorSyntax.Initializer?.Value is ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
            {
                foreach (var symbol1 in GetSymbols(objectCreationExpressionSyntax))
                    yield return symbol1;
            }
            else if (variableDeclaratorSyntax.Initializer?.Value is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                var symbol = GetSymbol(invocationExpressionSyntax.Expression);
                if (symbol is not null && symbol.DeclaringSyntaxReferences.Any())
                {
                    var def = (MethodDeclarationSyntax)symbol.DeclaringSyntaxReferences.First().GetSyntax();
                    var results = FindInstanciatedSymbolsFromMethodBody(def);
                    foreach (var result in results)
                    {
                        yield return result;
                    }
                }
            }
        }
    }

    private IEnumerable<ISymbol> GetSymbols(ExpressionStatementSyntax expressionStatementSyntax)
    {
        try
        {
            var symbol = GetSymbol(expressionStatementSyntax.Expression);
            if (symbol is null || !symbol.DeclaringSyntaxReferences.Any())
            {
                return Enumerable.Empty<ISymbol>();
            }
            var declarationSyntax = symbol.DeclaringSyntaxReferences.First().GetSyntax();
            if (declarationSyntax is MethodDeclarationSyntax methodDeclarationSyntax)
            {
                return FindInstanciatedSymbolsFromMethodBody(methodDeclarationSyntax);
            }
            return Enumerable.Empty<ISymbol>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Enumerable.Empty<ISymbol>();
        }
    }

    private IEnumerable<ISymbol> GetSymbols(ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
    {
        ISymbol? symbol = null;

        try
        {
            symbol = GetSymbol(objectCreationExpressionSyntax.Type);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }

        if (symbol is not null && symbol.DeclaringSyntaxReferences.Any())
        {
            yield return symbol;
        }
    }

    private ISymbol? GetSymbol(ExpressionSyntax expression) => _semanticModels[expression.SyntaxTree].GetSymbolInfo(expression).Symbol;

    private ISymbol? GetSymbol(SyntaxNode syntaxNode) => _semanticModels[syntaxNode.SyntaxTree].GetDeclaredSymbol(syntaxNode);
}

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