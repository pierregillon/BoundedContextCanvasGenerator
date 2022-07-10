namespace Catalog.Domain;

public interface ICatalogItemRepository
{
    Task<CatalogItem> Load(CatalogId id);
    Task Save(CatalogItem item);
}