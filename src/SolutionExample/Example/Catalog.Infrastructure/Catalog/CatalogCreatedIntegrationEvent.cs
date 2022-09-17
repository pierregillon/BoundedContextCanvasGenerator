namespace Catalog.Infrastructure.Catalog;

internal record CatalogCreatedIntegrationEvent(Guid Id, string CatalogName, string Description) : IIntegrationEvent;