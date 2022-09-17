namespace Catalog.Application;

public interface IDomainEventListener<in T>
{
    Task On(T @event);
}