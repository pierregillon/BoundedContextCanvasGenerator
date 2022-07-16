using Catalog.Domain.Catalog;
using Catalog.Domain.Items;

namespace Catalog.Infrastructure
{
    public class SqlCatalogItemRepository : ICatalogItemRepository
    {
        public Task<CatalogItem> Get(CatalogItemId id) => throw new NotImplementedException();

        public Task<IReadOnlyCollection<CatalogItem>> GetAll(CatalogId eventId) => throw new NotImplementedException();

        public Task Save(CatalogItem item) => throw new NotImplementedException();

        public Task Save(IEnumerable<CatalogItem> items) => throw new NotImplementedException();

        public Task<ItemCatalog> GetCatalog(CatalogId catalogId) => throw new NotImplementedException();
    }
}
