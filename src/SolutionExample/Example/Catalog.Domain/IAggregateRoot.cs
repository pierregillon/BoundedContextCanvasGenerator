namespace Catalog.Domain;

public interface IAggregateRoot<out TId>
{
    public TId Id { get; }
    public IReadOnlyCollection<IDomainEvent> UncommittedEvents { get; }

    public void StoreEvent(IDomainEvent domainEvent);
}