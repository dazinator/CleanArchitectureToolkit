namespace DomainEvents.Tests.DomainModel;

public class DomainEntity : IDomainEntity
{
    public void AddDomainEvent(IDomainEvent eventItem)
    {
        DomainEventsContext.Enqueue(eventItem);
    }
}