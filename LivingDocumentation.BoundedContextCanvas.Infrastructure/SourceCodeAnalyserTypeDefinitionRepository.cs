using Buildalyzer;
using Buildalyzer.Workspaces;
using LivingDocumentation.BoundedContextCanvas.Domain;
using Microsoft.CodeAnalysis;

namespace LivingDocumentation.BoundedContextCanvas.Infrastructure;

public class SourceCodeAnalyserTypeDefinitionRepository : ITypeDefinitionRepository
{
    public async IAsyncEnumerable<TypeDefinition> GetAll(SolutionName name)
    {
        var manager = new AnalyzerManager(name.Value);

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

            var typeDefinitions = new List<TypeDefinition>();

            new SourceCodeVisitor(semanticModel, typeDefinitions).Visit(await tree.GetRootAsync());

            foreach (var typeDefinition in typeDefinitions) {
                yield return typeDefinition;
            }
        }
    }
}