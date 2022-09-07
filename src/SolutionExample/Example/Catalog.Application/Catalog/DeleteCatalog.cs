using Catalog.Domain.Catalog;

namespace Catalog.Application.Catalog;

public record DeleteCatalogCommand(CatalogId Id) : ICommand;

public class DeleteCatalogHandler : ICommandHandler<DeleteCatalogCommand>
{
    private readonly ICatalogRepository _repository;

    public DeleteCatalogHandler(ICatalogRepository repository) => _repository = repository;

    public async Task Handle(DeleteCatalogCommand command)
    {
        var catalog = await _repository.Get(command.Id);

        catalog.Delete();

        await _repository.Save(catalog);
    }
}