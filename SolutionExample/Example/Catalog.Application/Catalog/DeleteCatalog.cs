using Catalog.Domain.Catalog;

namespace Catalog.Application.Catalog;

public record DeleteCatalog(CatalogId Id) : ICommand;

public class DeleteCatalogHandler : ICommandHandler<DeleteCatalog>
{
    private readonly ICatalogRepository _repository;

    public DeleteCatalogHandler(ICatalogRepository repository) => _repository = repository;

    public async Task Handle(DeleteCatalog command)
    {
        var catalog = await _repository.Get(command.Id);

        catalog.Delete();

        await _repository.Save(catalog);
    }
}