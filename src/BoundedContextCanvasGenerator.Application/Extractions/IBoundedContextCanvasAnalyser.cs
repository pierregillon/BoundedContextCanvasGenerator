using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public interface IBoundedContextCanvasAnalyser
{
    Task<BoundedContextCanvas> Analyse(SolutionPath solutionPath, ICanvasSettings settings);
}