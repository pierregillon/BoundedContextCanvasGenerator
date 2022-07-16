namespace Catalog.Domain.Catalog.Events;

public record CatalogDeleted(CatalogId Id) : IDomainEvent;