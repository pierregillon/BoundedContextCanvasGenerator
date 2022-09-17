namespace Catalog.Domain.Catalog.Events;

public record CatalogRegistered(CatalogId Id, CatalogName CatalogName, CatalogDescription Description) : IDomainEvent;