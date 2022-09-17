namespace Catalog.Infrastructure;

internal interface IBusListener<in T>
{
    Task On(T @event);
}