namespace Catalog.Infrastructure;

internal interface IBus
{
    Task Publish(IIntegrationEvent @event);
}