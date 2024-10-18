namespace DomainEvents.EFCore;
using Microsoft.EntityFrameworkCore;

public class SaveChangesCallbacks<TDbContext> : ISaveChangesCallbacks<TDbContext>
    where TDbContext : DbContext
{
    private readonly List<Func<CancellationToken, Task>> _afterSaveCallbacks = new();


    public void RegisterAfterSaveChangesAsyncCallback(Func<CancellationToken, Task> callback)
    {
        _afterSaveCallbacks.Add((callback));
    }

    public async Task InvokeAfterSaveChangesCallbacksAsync(CancellationToken cancellationToken)
    {
        foreach (var callback in _afterSaveCallbacks)
        {
            // Use dynamic typing to bypass compile-time type checking
            await callback(cancellationToken);
        }

        _afterSaveCallbacks.Clear(); // Ensure callbacks are only called once
    }
}