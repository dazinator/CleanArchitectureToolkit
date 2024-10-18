namespace DomainEvents;

public class DomainEventsService : IDomainEventsService
{
    private readonly IDomainEventPublisher _publisher;

    public DomainEventsService(IDomainEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        int maxPasses = 5;
        HashSet<IDomainEvent> processedEvents = new HashSet<IDomainEvent>();

        for (int i = 0; i < maxPasses; i++)
        {
            var eventsToProcess = DomainEventsContext.GetUnprocessedEvents()
                .Where(e => !processedEvents.Contains(e))
                .ToList();

            if (!eventsToProcess.Any())
            {
                break; // No more new events to process
            }

            foreach (var domainEvent in eventsToProcess)
            {
                await _publisher.SendAsync(domainEvent, cancellationToken);
                processedEvents.Add(domainEvent);
            }

            if (i == maxPasses - 1 && DomainEventsContext.GetUnprocessedEvents().Except(processedEvents).Any())
            {
                throw new Exception("Recursion limit exceeded processing domain events.");
            }
        }
    }

    public Task ClearAsync(CancellationToken cancellationToken)
    {
        DomainEventsContext.Clear();
        return Task.CompletedTask;
    }
}
