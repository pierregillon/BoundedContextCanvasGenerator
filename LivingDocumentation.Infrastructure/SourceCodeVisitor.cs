using LivingDocumentation.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LivingDocumentation.Infrastructure;

public class SourceCodeVisitor : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    private readonly List<TypeDefinition> _typeDefinitions;

    public SourceCodeVisitor(SemanticModel semanticModel, List<TypeDefinition> typeDefinitions)
    {
        this._semanticModel = semanticModel;
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
            _typeDefinitions.Add(new TypeDefinition(
                type.GetFullName(),
                Enumerable.Select(type.AllInterfaces, i => i.GetFullName()).ToArray()
            ));
        }
    }
}