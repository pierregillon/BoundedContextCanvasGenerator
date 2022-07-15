using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public interface ITypeDefinitionExtractor
{
    Task<TypeDefinitionExtraction> Extract(SolutionPath solutionPath, ICanvasSettings settings);
}