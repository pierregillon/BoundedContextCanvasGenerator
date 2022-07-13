using Catalog.Domain.Items.Events;

namespace Catalog.Domain.Items;

public class CatalogItem : AggregatorRoot<CatalogItemId>
{
    public CatalogId CatalogId { get; }
    public Title Title { get; }
    public Price Price { get; }

    public static CatalogItem Add(CatalogId catalogId, Title title, Price price)
    {
        var catalogItem = new CatalogItem(CatalogItemId.New(), catalogId, title, price);
        catalogItem.StoreEvent(new CatalogItemAdded(catalogItem.Id, catalogId, catalogItem.Title, catalogItem.Price));
        return catalogItem;
    }

    public CatalogItem(CatalogItemId id, CatalogId catalogId, Title title, Price price) : base(id)
    {
        Id = id;
        CatalogId = catalogId;
        Title = title;
        Price = price;
    }

    public void Entitle(Title newTitle)
    {
        if (this.Title != newTitle)
        {
            this.StoreEvent(new CatalogItemEntitled(this.Id, this.Title, newTitle));
        }
    }

    public void AdjustPrice(Price newPrice)
    {
        if (this.Price != newPrice)
        {
            this.StoreEvent(new CatalogItemPriceAdjusted(this.Id, this.Price, newPrice));
        }
    }

    public void Remove()
    {
        this.StoreEvent(new CatalogItemRemoved(this.Id));
    }
}