namespace Catalog.Domain.Items.Events;

public record CatalogItemAdded(CatalogItemId Id, CatalogId CatalogId, Title Title, Price Price) : IDomainEvent;