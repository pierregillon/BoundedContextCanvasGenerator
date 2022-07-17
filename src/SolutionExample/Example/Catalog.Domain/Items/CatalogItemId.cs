namespace Catalog.Domain.Items;

public record CatalogItemId(Guid Value)
{
    public static CatalogItemId New() => new(Guid.NewGuid());
}