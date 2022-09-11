using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public interface IBoundedContextCanvasAnalyser
{
    Task<BoundedContextCanvas> Analyse(TypeDefinitionExtract typeDefinitionExtract, ICanvasSettings canvasSettings);
}