namespace DomainEvents;
using System.Diagnostics.CodeAnalysis;

public static class DomainEventsContext
{
    private static readonly AsyncLocal<List<IDomainEvent>> _events = new();

    public static List<IDomainEvent> Events => _events.Value ??= new List<IDomainEvent>();

    public static void Enqueue(IDomainEvent domainEvent)
    {
        Events.Add(domainEvent);
    }

    public static IReadOnlyList<IDomainEvent> GetUnprocessedEvents()
    {
        return Events.AsReadOnly();
    }

    public static void Clear()
    {
        Events.Clear();
    }

    public static bool Any()
    {
        return Events.Any();
    }

    public static bool TryPeek(out IDomainEvent domainEvent)
    {
        if (_events.Value == null || !Events.Any())
        {
            domainEvent = null;
            return false;
        }

        domainEvent = Events[0];
        return true;
    }
}
