namespace DomainEvents;

public interface IDomainEventsService
{
    Task ProcessAsync(CancellationToken cancellationToken = default);

    Task ClearAsync(CancellationToken cancellationToken);
}
