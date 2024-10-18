namespace DomainEvents.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class EnsureDomainEventsAreProcessedBeforeSaveChangesInterceptor<TDbContext> : SaveChangesInterceptor
    where TDbContext : DbContext
{
    private readonly ISaveChangesCallbacks<TDbContext> _saveChangesCallbacks;

    public EnsureDomainEventsAreProcessedBeforeSaveChangesInterceptor(ISaveChangesCallbacks<TDbContext> saveChangesCallbacks)
    {
        _saveChangesCallbacks = saveChangesCallbacks;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        // Logic before saving changes
        // lets put in a guard rail to ensure all domain events have been dispatched.
        if (DomainEventsContext.Any())
        {
            throw new Exception("Domain events have not been dispatched before save changes.");
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        // Async logic before saving changes
        // lets put in a guard rail to ensure all domain events have been dispatched.
        if (DomainEventsContext.Any())
        {
            throw new Exception("Domain events have not been dispatched before save changes.");
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        // Async logic after saving changes
        // await _processor.ProcessAfterSaveAsync(eventData.Context.ChangeTracker, cancellationToken);
        await _saveChangesCallbacks.InvokeAfterSaveChangesCallbacksAsync(cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}