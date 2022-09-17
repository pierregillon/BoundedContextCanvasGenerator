using Catalog.Application;
using Catalog.Domain.Catalog.Events;

namespace Catalog.Infrastructure.Catalog;

internal class PublishCatalogCreatedIntegrationEvent : IDomainEventListener<CatalogRegistered>
{
    private readonly IBus _bus;

    public PublishCatalogCreatedIntegrationEvent(IBus bus) => _bus = bus;

    public async Task On(CatalogRegistered @event)
    {
        var integrationEVent = new CatalogCreatedIntegrationEvent(
            @event.Id.Value,
            @event.CatalogName.Value,
            @event.Description.Value
        );

        await _bus.Publish(integrationEVent);
    }
}