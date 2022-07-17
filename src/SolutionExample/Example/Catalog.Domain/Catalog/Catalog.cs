using Catalog.Domain.Catalog.Events;

namespace Catalog.Domain.Catalog;

/// <summary>
/// An enumeration of items to purchase. It is systematically described and target a specific audience.
/// </summary>
public class Catalog : AggregatorRoot<CatalogId>
{
    private readonly CatalogDescription _description;

    private Catalog(CatalogId id, CatalogDescription description) : base(id) => _description = description;

    public static Catalog Register(CatalogDescription description)
    {
        var catalog = new Catalog(CatalogId.New(), description);
        catalog.StoreEvent(new CatalogRegistered(catalog.Id, description));
        return catalog;
    }

    public void Delete()
    {
        this.StoreEvent(new CatalogDeleted(this.Id));
    }
}