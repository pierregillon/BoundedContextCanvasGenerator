namespace Catalog.Infrastructure.Order;

internal record ItemOrderedIntegrationEvent(Guid ItemId, int OrderedQuantity);