namespace Catalog.Domain.Catalog;

public interface ICatalogRepository
{
    Task<Catalog> Get(CatalogId id);
    Task Save(Catalog catalog);
}