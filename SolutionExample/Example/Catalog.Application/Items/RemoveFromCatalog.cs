using Catalog.Domain.Items;

namespace Catalog.Application.Items;

public record RemoveFromCatalogCommand(CatalogItemId Id, Title NewTitle) : ICommand;

public class RemoveFromCatalogCommandHandler : ICommandHandler<RemoveFromCatalogCommand>
{
    private readonly ICatalogItemRepository _repository;

    public RemoveFromCatalogCommandHandler(ICatalogItemRepository repository) => _repository = repository;

    public async Task Handle(RemoveFromCatalogCommand command)
    {
        var catalogItem = await _repository.Get(command.Id);

        catalogItem.Remove();

        await _repository.Save(catalogItem);
    }
}