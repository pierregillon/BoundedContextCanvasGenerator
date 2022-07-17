namespace Catalog.Domain.Items.Events;

public record CatalogItemPriceAdjusted(CatalogItemId Id, Price Before, Price After) : IDomainEvent;