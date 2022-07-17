using Catalog.Domain.Catalog;

namespace Catalog.Domain.Items;

public interface ICatalogItemRepository
{
    Task<CatalogItem> Get(CatalogItemId id);
    Task<IReadOnlyCollection<CatalogItem>> GetAll(CatalogId eventId);
    Task Save(CatalogItem item);
    Task Save(IEnumerable<CatalogItem> items);
    Task<ItemCatalog> GetCatalog(CatalogId catalogId);
}