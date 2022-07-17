namespace Catalog.Application;

internal interface IDomainEventListener<in T>
{
    Task On(T @event);
}