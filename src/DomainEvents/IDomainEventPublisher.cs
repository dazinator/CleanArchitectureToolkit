namespace DomainEvents;

public interface IDomainEventPublisher
{
    Task SendAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
    void Send(IDomainEvent domainEvent);
}