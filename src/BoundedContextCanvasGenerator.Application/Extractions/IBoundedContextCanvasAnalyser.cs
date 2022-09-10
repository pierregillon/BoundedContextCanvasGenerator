using BoundedContextCanvasGenerator.Domain.BC;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public interface IBoundedContextCanvasAnalyser
{
    Task<BoundedContextCanvas> Analyse(TypeDefinitionExtract typeDefinitionExtract);
}