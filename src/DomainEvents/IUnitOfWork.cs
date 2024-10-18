namespace DomainEvents;

/// <summary>
/// A default contract for a unit of work.
/// </summary>
/// <remarks>You don't have to use this contract in your application you can create your own. However be sure that any unit of work implementation that you do create, calls <see cref="IDomainEventsService.ProcessAsync"/> to include changes from domain event handling in the transaction.</remarks>
public interface IUnitOfWork
{
    Task CompleteAsync(CancellationToken cancellationToken = default);
}