namespace Catalog.Domain.Catalog;

public record CatalogId(Guid Value)
{
    public static CatalogId New() => new(Guid.NewGuid());
}