namespace Catalog.Domain.Catalog;

public record CatalogDescription(string Value)
{
    public static CatalogDescription Empty => new(string.Empty);
}