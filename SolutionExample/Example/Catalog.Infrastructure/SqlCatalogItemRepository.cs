using Catalog.Domain;
using Catalog.Domain.Items;

namespace Catalog.Infrastructure
{
    public class SqlCatalogItemRepository : ICatalogItemRepository
    {
        public Task<CatalogItem> Load(CatalogItemId id)
        {
            throw new NotImplementedException();
        }

        public Task Save(CatalogItem item)
        {
            throw new NotImplementedException();
        }
    }
}
