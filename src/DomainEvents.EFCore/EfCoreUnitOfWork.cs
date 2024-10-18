namespace DomainEvents.EFCore;

using System;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// An optional <see cref="IUnitOfWork"/> implementation for EF Core that will ensure domain events are processed within a resilient transaction.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <remarks>You don't have to use this as a dependency in your application but if you roll your own solution, you should ensure you call <see cref="IDomainEventsService.ProcessAsync"/> and <see cref="IDomainEventsService.ClearAsync();"/> </remarks>
public class EfCoreUnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    private readonly TDbContext _context;
    private readonly IDomainEventsService _domainEventsService;

    public EfCoreUnitOfWork(TDbContext context, IDomainEventsService domainEventsService)
    {
        _context = context;
        _domainEventsService = domainEventsService;
    }

    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        int retryCount = 0;
        // strategy.ExecuteInTransaction
        await strategy.ExecuteAsync(async () =>
        {
            if (retryCount > 0 && await WasTransactionCommitted(cancellationToken))
            {
                // we have recovered - the transaction has already been committed
                await TransactionCompleted(cancellationToken);
                return;
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _domainEventsService.ProcessAsync(cancellationToken);
                await _context.SaveChangesAsync(false, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                await TransactionCompleted(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // Re-throw the exception to allow the execution strategy to handle it
            }
        });
    }

    private async Task TransactionCompleted(CancellationToken cancellationToken)
    {

        // Clear all events after successful transaction
        await _domainEventsService.ClearAsync(cancellationToken);
        _context.ChangeTracker.AcceptAllChanges();
        OnTransactionCompletedAsync(cancellationToken);
    }

    protected void OnTransactionCompletedAsync(CancellationToken cancellationToken)
    {
        // open for extension.
    }

    /// <summary>
    /// Implement a method to check whether an errored transaction was actually committed. This could happen due to a connection error at the commit phase of the transaciton leaving the application in an indeterminate state.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>Default implementation always returns false</remarks>
    protected virtual async Task<bool> WasTransactionCommitted(CancellationToken cancellationToken)
    {
        // Default implementation always returns false
        // Override this in derived classes to implement custom retry check logic
        return await Task.FromResult(false);
    }

}
