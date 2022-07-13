namespace Catalog.Domain.Items.Events;

public record CatalogItemEntitled(CatalogItemId Id, Title Before, Title After) : IDomainEvent;