namespace Catalog.Domain.Items.Events;

public record CatalogItemRemoved(CatalogItemId Id) : IDomainEvent;