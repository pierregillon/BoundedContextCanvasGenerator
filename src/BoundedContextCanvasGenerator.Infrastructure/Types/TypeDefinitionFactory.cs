using BoundedContextCanvasGenerator.Domain.Types;
using Microsoft.CodeAnalysis;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class TypeDefinitionFactory
{
    public async Task<IReadOnlyCollection<TypeDefinition>> Build(IEnumerable<Compilation> compilations)
    {
        var results = GetSemanticModels(compilations);

        var typeDefinitions = await ParseSourceCodeForTypeDefinitions(results);

        var methodDefinitions = await ParseSourceCodeForMethodDefinitions(results);

        return Merge(typeDefinitions, methodDefinitions).ToArray();
    }

    private static IReadOnlyCollection<(SyntaxTree SyntaxTree, SemanticModel SemanticModel)> GetSemanticModels(IEnumerable<Compilation> compilations)
    {
        return compilations
            .SelectMany(c => c.SyntaxTrees.Select(x => new { Compilation = c, SyntaxTree = x }))
            .Select(x => (x.SyntaxTree, x.Compilation.GetSemanticModel(x.SyntaxTree, true)))
            .ToArray();
    }

    private static async Task<MethodDefinitions> ParseSourceCodeForMethodDefinitions(IReadOnlyCollection<(SyntaxTree SyntaxTree, SemanticModel SemanticModel)> results)
    {
        var methodDefinitions = new MethodDefinitions();
        var methodSourceCodeVisitor = new MethodSourceCodeVisitor(results.Select(x => x.SemanticModel), methodDefinitions);
        foreach (var result in results) {
            methodSourceCodeVisitor.Visit(await result.SyntaxTree.GetRootAsync());
        }
        return methodDefinitions;
    }

    private static async Task<List<TypeDefinition>> ParseSourceCodeForTypeDefinitions(IReadOnlyCollection<(SyntaxTree SyntaxTree, SemanticModel SemanticModel)> results)
    {
        var typeDefinitions = new List<TypeDefinition>();
        foreach (var (syntaxTree, semanticModel) in results) {
            new ClassSourceCodeVisitor(semanticModel, typeDefinitions).Visit(await syntaxTree.GetRootAsync());
        }
        return typeDefinitions;
    }

    private static IEnumerable<TypeDefinition> Merge(IReadOnlyCollection<TypeDefinition> typeDefinitions, MethodDefinitions methodDefinitions)
    {
        var allTypes = typeDefinitions.ToDictionary(x => x.FullName);

        foreach (var typeDefinition in typeDefinitions) {
            var instanciators = methodDefinitions
                .FindInstanciators(typeDefinition)
                .Select(x => new Instanciator(allTypes[x.Item1], x.Item2))
                .ToArray();

            if (instanciators.Any()) {

                // WARNING : we are creating a new TypeDefinition so the allTypes dictionary
                // does not contains this instance anymore. It can leads to multiple instances
                // with different information.

                yield return typeDefinition with { Instanciators = instanciators }; 
            }
            else {
                yield return typeDefinition;
            }
        }
    }
}