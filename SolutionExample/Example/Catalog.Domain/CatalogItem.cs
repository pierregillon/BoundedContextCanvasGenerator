namespace Catalog.Domain;

public class CatalogItem : IAggregateRoot
{
    public static CatalogItem New(string name, Price price)
    {
        throw new NotImplementedException();
    }
}

public record Price(Amount Amount, Currency Currency);

public record Currency(string IsoValue);

public record Amount(double Value);