namespace Catalog.Domain.Items;

public interface ICatalogItemRepository
{
    Task<CatalogItem> Load(CatalogItemId id);
    Task Save(CatalogItem item);
    Task<ItemCatalog> GetCatalog(CatalogId catalogId);
}