using Catalog.Domain.Catalog.Events;
using Catalog.Domain.Items;

namespace Catalog.Application.Items;

internal class RemoveAllItemsOnCatalogDeleted : IDomainEventListener<CatalogDeleted>
{
    private readonly ICatalogItemRepository _repository;

    public RemoveAllItemsOnCatalogDeleted(ICatalogItemRepository repository) => _repository = repository;

    public async Task On(CatalogDeleted @event)
    {
        var items = await this._repository.GetAll(@event.Id);

        foreach (var item in items) {
            item.Remove();
        }

        await _repository.Save(items);
    }
}