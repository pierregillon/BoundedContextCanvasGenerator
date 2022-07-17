namespace Catalog.Domain.Catalog.Events;

public record CatalogRegistered(CatalogId Id, CatalogDescription Description) : IDomainEvent;