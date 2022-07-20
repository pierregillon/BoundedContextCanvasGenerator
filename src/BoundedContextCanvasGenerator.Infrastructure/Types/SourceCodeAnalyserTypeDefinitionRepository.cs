using BoundedContextCanvasGenerator.Domain.Types;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class SourceCodeAnalyserTypeDefinitionRepository : ITypeDefinitionRepository
{
    public async IAsyncEnumerable<TypeDefinition> GetAll(SolutionPath path)
    {
        var manager = new AnalyzerManager(path.Value);

        var workspace = manager.GetWorkspace();

        foreach (var project in workspace.CurrentSolution.Projects) {
            await foreach (var typeDefinition in GetTypeDefinitions(project)) {
                yield return typeDefinition;
            }
        }
    }

    private static async IAsyncEnumerable<TypeDefinition> GetTypeDefinitions(Project project)
    {
        var compilation = await project.GetCompilationAsync();

        if (compilation == null) {
            yield break;
        }

        foreach (var tree in compilation.SyntaxTrees) {
            var semanticModel = compilation.GetSemanticModel(tree, true);

            var visitedData = new VisitedData();

            new SourceCodeVisitor(semanticModel, visitedData).Visit(await tree.GetRootAsync());

            foreach (var typeDefinition in visitedData.TypeDefinitions) {
                yield return typeDefinition;
            }
        }
    }
}