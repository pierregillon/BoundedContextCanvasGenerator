namespace Catalog.Domain;

public abstract class AggregatorRoot<T> : IAggregateRoot<T>
{
    private readonly List<IDomainEvent> _uncommittedEvents = new();

    public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _uncommittedEvents;

    protected AggregatorRoot(T id) => Id = id;

    public T Id { get; protected set; }

    public void StoreEvent(IDomainEvent domainEvent) => _uncommittedEvents.Add(domainEvent);
}