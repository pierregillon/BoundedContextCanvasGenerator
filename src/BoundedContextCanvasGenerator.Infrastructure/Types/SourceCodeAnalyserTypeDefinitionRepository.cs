using System.Transactions;
using BoundedContextCanvasGenerator.Domain;
using BoundedContextCanvasGenerator.Domain.Types;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class SourceCodeAnalyserTypeDefinitionRepository : ITypeDefinitionRepository
{
    private readonly TypeDefinitionFactory _factory;

    public SourceCodeAnalyserTypeDefinitionRepository(TypeDefinitionFactory factory) => _factory = factory;

    public async IAsyncEnumerable<TypeDefinition> GetAll(SolutionPath path)
    {
        var manager = new AnalyzerManager(path.Value);

        var workspace = manager.GetWorkspace();

        var compilations = await workspace.CurrentSolution.Projects
            .Select(x => x.GetCompilationAsync())
            .Pipe(Task.WhenAll);

        foreach (var result in await _factory.Build(compilations!)) {
            yield return result;
        }
    }

}

public class TypeDefinitionFactory
{
    public async Task<IReadOnlyCollection<TypeDefinition>> Build(IEnumerable<Compilation> compilations)
    {
        var results = compilations
            .SelectMany(c => c.SyntaxTrees.Select(x => new { Compilation = c, SyntaxTree = x }))
            .Select(x => new { x.SyntaxTree, SemanticModel = x.Compilation.GetSemanticModel(x.SyntaxTree, true) })
            .ToArray();

        var visitedData = new VisitedData();

        foreach (var result in results) {
            
            new SourceCodeVisitor(result.SemanticModel, visitedData).Visit(await result.SyntaxTree.GetRootAsync());
        }

        var visited = new VisitedData2();
        var visitor = new SourceCodeMethodVisitor(results.Select(x => x.SemanticModel), visited);
        foreach (var result in results)
        {
           visitor.Visit(await result.SyntaxTree.GetRootAsync());
        }

        return Merge(visitedData, visited).ToArray();
    }

    private IEnumerable<TypeDefinition> Merge(VisitedData visitedData, VisitedData2 visited)
    {
        var allTypes = visitedData.TypeDefinitions.ToDictionary(x => x.FullName);

        foreach (var typeDefinition in visitedData.TypeDefinitions) {
            var instanciators = visited.Methods
                .Where(x => x.Value.Any(m => m.InstanciatedTypes.Contains(typeDefinition.FullName)))
                .SelectMany(x => x.Value.Select(m => new Instanciator(allTypes[x.Key], new MethodName(m.Name))))
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