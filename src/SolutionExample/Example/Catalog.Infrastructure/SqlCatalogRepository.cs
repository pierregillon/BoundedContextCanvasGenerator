using Catalog.Domain.Catalog;
using Catalog.Domain.Items;

namespace Catalog.Infrastructure;

public class SqlCatalogRepository : ICatalogRepository
{
    public Task<Domain.Catalog.Catalog> Get(CatalogId id) => throw new NotImplementedException();

    public Task Save(Domain.Catalog.Catalog catalog) => throw new NotImplementedException();
}