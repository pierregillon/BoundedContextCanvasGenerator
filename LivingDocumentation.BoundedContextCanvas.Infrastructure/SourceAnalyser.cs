using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class SourceAnalyser : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    private readonly List<TypeDefinition> _typeDefinitions;

    public SourceAnalyser(SemanticModel semanticModel, List<TypeDefinition> typeDefinitions)
    {
        this._semanticModel = semanticModel;
        _typeDefinitions = typeDefinitions;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var type = _semanticModel.GetDeclaredSymbol(node);
        
        if (type != null) {
            _typeDefinitions.Add(new TypeDefinition(
                new TypeFullName(type.Name), 
                Enumerable.Select(type.AllInterfaces, i => new TypeFullName(i.Name))
            ));
        }

        base.VisitClassDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        var type = _semanticModel.GetDeclaredSymbol(node);

        if (type != null)
        {
            _typeDefinitions.Add(new TypeDefinition(
                new TypeFullName(type.Name),
                Enumerable.Select(type.AllInterfaces, i => new TypeFullName(i.Name))
            ));
        }

        base.VisitRecordDeclaration(node);
    }
}