namespace Catalog.Domain.Catalog.Events;

public record CatalogRegistered(CatalogId Id, CatalogName catalogName, CatalogDescription Description) : IDomainEvent;