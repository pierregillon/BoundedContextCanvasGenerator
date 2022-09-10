using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public interface ITypeDefinitionExtractor
{
    public Task<TypeDefinitionExtract> Extract(SolutionPath solutionPath, ICanvasSettings settings);
}