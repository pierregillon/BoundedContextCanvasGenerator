using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.Types;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public class BoundedContextCanvasSourceCodeAnalyser : IBoundedContextCanvasAnalyser
{
    private readonly ITypeDefinitionRepository _repository;

    public BoundedContextCanvasSourceCodeAnalyser(ITypeDefinitionRepository repository) => _repository = repository;

    public async Task<BoundedContextCanvas> Analyse(TypeDefinitionExtract typeDefinitionExtract)
    {
        throw new NotImplementedException();
    }
}