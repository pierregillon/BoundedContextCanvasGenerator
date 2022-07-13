namespace Catalog.Domain;

public interface IAggregateRoot<out TId>
{
    public TId Id { get; }

    public void StoreEvent(IDomainEvent domainEvent);
}