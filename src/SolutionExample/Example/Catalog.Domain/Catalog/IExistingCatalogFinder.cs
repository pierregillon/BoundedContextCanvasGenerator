namespace Catalog.Domain.Catalog;

public interface IExistingCatalogFinder
{
    Task<bool> AnyCatalogWithName(CatalogName name);
}