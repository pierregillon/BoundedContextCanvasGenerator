namespace Catalog.Domain.Items;

public record ItemCatalog(CatalogId Id)
{
    public CatalogItem Add(Title title, Price price) => CatalogItem.Add(this.Id, title, price);
}