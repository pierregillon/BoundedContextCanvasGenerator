using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class ClassSourceCodeVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    private readonly List<TypeDefinition> _typeDefinitions;

    public ClassSourceCodeVisitor(SemanticModel semanticModel, List<TypeDefinition> typeDefinitions)
    {
        _semanticModel = semanticModel;
        _typeDefinitions = typeDefinitions;
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
            _typeDefinitions.Add(type.ToTypeDefinition());
        }
    }
}