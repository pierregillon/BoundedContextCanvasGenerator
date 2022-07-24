using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public class SourceCodeCompiler
{
    public Compilation Compile(params string[] sourceCodes)
    {
        sourceCodes
            .Should()
            .AllSatisfy(x => x.Should().NotBeNullOrEmpty("without source code there is nothing to test"));

        var trees = sourceCodes
            .Select(x => x.Trim())
            .Select(x => CSharpSyntaxTree.ParseText(x))
            .ToArray();

        return CSharpCompilation.Create("Test")
            .WithOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithAllowUnsafe(true)
            )
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(trees);
    }
}