namespace Catalog.Domain.Items.Events;

public record CatalogItemQuantityAdjusted(CatalogItemId Id, Quantity Quantity) : IDomainEvent;